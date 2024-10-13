using Microsoft.AspNetCore.Mvc;
using BookingService.Data;
using BookingService.Models;
using BookingService.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/businesses/{businessId}/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly BookingServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly BusinessValidationService _businessValidateService;

        public EmployeeController(BookingServiceDbContext context, IMapper mapper, UserService userService, BusinessValidationService businessValidationService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _businessValidateService = businessValidationService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddEmployee(int businessId, [FromBody] EmployeeCreateDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                if (!await _userService.IsOwnerOrAdminAsync(userId.Value, businessId))
                {
                    return Forbid();
                }

                var employee = _mapper.Map<Employee>(employeeDto);
                employee.BusinessId = businessId;

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Pracownik został pomyślnie dodany", employeeId = employee.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(int businessId)
        {
            try
            {
                var business = await _context.Businesses.Include(b => b.Employees).ThenInclude(e => e.WorkingHours).FirstOrDefaultAsync(b => b.Id == businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(business.Employees);
                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEmployee(int businessId, int id, [FromBody] EmployeeUpdateDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                if (!await _userService.IsOwnerOrAdminAsync(userId.Value, businessId))
                {
                    return Forbid();
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null || employee.BusinessId != businessId)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika lub ne jest on przepisany do tego biznesu" });
                }

                _mapper.Map(employeeDto, employee);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Dane pracownika zostały zaktualizowane" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEmployee(int businessId, int id)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                if (!await _userService.IsOwnerOrAdminAsync(userId.Value, businessId))
                {
                    return Forbid();
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null || employee.BusinessId != businessId)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika lub nie jest on przypisany do tego biznesu" });
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                var isPublished = await _businessValidateService.ValidateBusinessIsPublished(businessId);

                if (!isPublished)
                {
                    await _businessValidateService.UnpublishBusinessAsync(businessId);
                    return Ok(new { message = "Pracownik został usunięty, biznes został niepublikowany" });
                }

                return Ok(new { message = "Pracownik został usunięty" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPost("{id}/upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(int businessId, int id, IFormFile file)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                if (!await _userService.IsOwnerOrAdminAsync(userId.Value, businessId))
                {
                    return Forbid();
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null || employee.BusinessId != businessId)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika lub nie jest on przypisany do tego biznesu" });
                }

                var googleCloudStorage = new GoogleCloudStorageService();
                var objectName = $"avatars/{userId}_{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    var fileUrl = await googleCloudStorage.UploadFileAsync(stream, objectName);

                    employee.AvatarUrl = fileUrl;
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Dodano zdjęcie profilu pracownika", avatarUrl = employee.AvatarUrl });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{id}/working-hours")]
        [Authorize]
        public async Task<IActionResult> UpdateWorkingHours(int businessId, int id, [FromBody] List<WorkingHoursUpdateDto> workingHoursDto)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                if (!await _userService.IsOwnerOrAdminAsync(userId.Value, businessId))
                {
                    return Forbid();
                }

                var employee = await _context.Employees.Include(e => e.WorkingHours).FirstOrDefaultAsync(e => e.Id == id);
                if (employee == null)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika" });
                }

                employee.WorkingHours.Clear();
                var workingHours = _mapper.Map<List<WorkingHours>>(workingHoursDto);
                employee.WorkingHours.AddRange(workingHours);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Godziny pracy zostały zaktualizowane" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{id}/working-hours")]
        [Authorize]
        public async Task<IActionResult> GetWorkingHours(int businessId, int id)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                if (!await _userService.IsOwnerOrAdminAsync(userId.Value, businessId))
                {
                    return Forbid();
                }

                var employee = await _context.Employees.Include(e => e.WorkingHours).FirstOrDefaultAsync(e => e.Id == id);
                if (employee == null || employee.BusinessId != businessId)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika lub nie jest on przypisany do tego biznesu" });
                }

                var workingHoursDto = _mapper.Map<IEnumerable<WorkingHoursDto>>(employee.WorkingHours);
                return Ok(workingHoursDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }
    }
}
