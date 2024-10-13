using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/businesses/{businessId}/services")]
    public class ServiceController : ControllerBase
    {
        private readonly BookingServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly BusinessValidationService _businessValidateService;

        public ServiceController(BookingServiceDbContext context, IMapper mapper, UserService userService, BusinessValidationService businessValidationService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _businessValidateService = businessValidationService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddService(int businessId, [FromBody] ServiceCreateDto serviceDto)
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

                var service = _mapper.Map<Service>(serviceDto);
                service.BusinessId = businessId;

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Usługa została pomyślnie dodana", serviceId = service.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServices(int businessId)
        {
            try
            {
                var business = await _context.Businesses.Include(b => b.Services).FirstOrDefaultAsync(b => b.Id == businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var serviceDtos = _mapper.Map<IEnumerable<ServiceDto>>(business.Services);
                return Ok(serviceDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int businessId, int id)
        {
            try
            {
                var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id && s.BusinessId == businessId);
                if (service == null)
                {
                    return NotFound(new { message = "Nie znaleziono usługi" });
                }

                var serviceDto = _mapper.Map<ServiceDto>(service);
                return Ok(serviceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateService(int businessId, int id, [FromBody] ServiceUpdateDto serviceDto)
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

                var service = await _context.Services.FindAsync(id);
                if (service == null || service.BusinessId != businessId)
                {
                    return NotFound(new { message = "Nie znaleziono usługi" });
                }
                
                _mapper.Map(serviceDto, service);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Usługa została zaktualizowana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteService(int businessId, int id)
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

                var service = await _context.Services.FindAsync(id);
                if (service == null || service.BusinessId != businessId)
                {
                    return NotFound(new { message = "Nie znaleziono usługi" });
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                var isPublished = await _businessValidateService.ValidateBusinessIsPublished(businessId);

                if (!isPublished)
                {
                    await _businessValidateService.UnpublishBusinessAsync(businessId);
                    return Ok(new { message = "Usługa została usunięta, biznes został niepublikowany" });
                }

                return Ok(new { message = "Usługa została usunięta " });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }
    }
}
