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
    [Route("api/businesses")]
    public class BusinessController : ControllerBase
    {
        private readonly BookingServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        private readonly BusinessValidationService _businessValidateService;

        public BusinessController(BookingServiceDbContext context, IMapper mapper, UserService userService, BusinessValidationService businessValidationService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _businessValidateService = businessValidationService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RegisterBusiness([FromBody] BusinessCreateDto businessDto)
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

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var business = _mapper.Map<Business>(businessDto);
                business.UserId = user.Id;

                user.Role = Roles.Owner;

                _context.Businesses.Add(business);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Biznes został pomyślnie utworzony", businessId = business.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBusiness()
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var business = await _context.Businesses
                    .Include(b => b.Address)
                    .Include(b => b.Images.Where(i => i.IsPrimary))
                    .FirstOrDefaultAsync(b => b.UserId == userId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var businessDto = _mapper.Map<BusinessDto>(business);
                return Ok(businessDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetBusinessById(int businessId)
        {
            try
            {
                var business = await _context.Businesses
                    .Include(b => b.Address)
                    .Include(b => b.WorkingHours)
                    .Include(b => b.Images)
                    .Include(b => b.Services)
                    .Include(b => b.Employees)
                    .FirstOrDefaultAsync(b => b.Id == businessId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var businessDto = _mapper.Map<BusinessDetailDto>(business);
                return Ok(businessDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{businessId}")]
        [Authorize]
        public async Task<IActionResult> UpdateBusiness(int businessId, [FromBody] BusinessUpdateDto businessDto)
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

                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                _mapper.Map(businessDto, business);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Dane przedsiębiorstwa zaktualizowane" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBusinesses([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string? category = null, [FromQuery] string? location = null, [FromQuery] string? searchTerms = null)
        {
            try
            {
                var query = _context.Businesses.Where(b => b.IsPublished).Include(b => b.Services.Where(s => s.IsFeatured)).Include(b => b.Images.Where(i => i.IsPrimary)).AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.AsEnumerable().Where(b => b.Category.ToLowerInvariant().Contains(category.ToLowerInvariant())).AsQueryable();
                }

                if (!string.IsNullOrEmpty(location))
                {
                    query = query.AsEnumerable().Where(b => b.Address.City.ToLowerInvariant().Contains(location.ToLowerInvariant())).AsQueryable();
                }

                if (!string.IsNullOrEmpty(searchTerms))
                {
                    query = query.AsEnumerable().Where(b => b.Name.ToLowerInvariant().Contains(searchTerms.ToLowerInvariant())).AsQueryable();
                }

                var totalRecords = query.Count();
                var businesses = query.Skip((page - 1) * limit).Take(limit).ToList();

                var businessDtos = _mapper.Map<List<BusinessListDto>>(businesses);

                return Ok(new
                {
                    businesses = businessDtos,
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = (int)Math.Ceiling((double)totalRecords / limit),
                        totalRecords
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{businessId}/working-hours")]
        [Authorize]
        public async Task<IActionResult> UpdateWorkingHours(int businessId, [FromBody] List<WorkingHoursUpdateDto> workingHoursDto)
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

                var business = await _context.Businesses.Include(b => b.WorkingHours).FirstOrDefaultAsync(b => b.Id == businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                _context.WorkingHours.RemoveRange(business.WorkingHours);

                var workingHours = _mapper.Map<List<WorkingHours>>(workingHoursDto);
                business.WorkingHours.AddRange(workingHours);


                await _context.SaveChangesAsync();

                var isPublished = await _businessValidateService.ValidateBusinessIsPublished(businessId);

                if (!isPublished)
                {
                    await _businessValidateService.UnpublishBusinessAsync(businessId);
                    return Ok(new { message = "Godziny pracy zaktualizowane, biznes został niepublikowany" });
                }

                return Ok(new { message = "Godziny pracy zaktualizowane" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}/working-hours")]
        public async Task<IActionResult> GetWorkingHours(int businessId)
        {
            try
            {
                var business = await _context.Businesses.Include(b => b.WorkingHours).FirstOrDefaultAsync(b => b.Id == businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var workingHoursDto = _mapper.Map<IEnumerable<WorkingHoursDto>>(business.WorkingHours);
                return Ok(workingHoursDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}/bookings")]
        [Authorize]
        public async Task<IActionResult> GetBusinessBookings(int businessId)
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

                var business = await _context.Businesses.FirstOrDefaultAsync(b => b.Id == businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var bookings = await _context.Bookings
                    .Include(b => b.Service)
                    .ThenInclude(s => s.Employee)
                    .ThenInclude(s => s.Business)
                    .Where(b => b.Service.BusinessId == business.Id)
                    .ToListAsync();

                var bookingListDtos = _mapper.Map<List<BookingListDto>>(bookings);
                return Ok(bookingListDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPost("{businessId}/upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadBusinessImage(int businessId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Nie udało się załadować pliku" });
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

                var business = await _context.Businesses.FirstOrDefaultAsync(b => b.Id == businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var googleCloudStorage = new GoogleCloudStorageService();
                var objectName = $"businessImages/{businessId}_{file.FileName}";

                using (var stream = file.OpenReadStream())
                {
                    var fileUrl = await googleCloudStorage.UploadFileAsync(stream, objectName);

                    var businessImage = new BusinessImage
                    {
                        BusinessId = businessId,
                        ImageUrl = fileUrl
                    };

                    _context.BusinessImages.Add(businessImage);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Dodano obraz do galerii firmy", imageUrl = businessImage.ImageUrl });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}/images")]
        public async Task<IActionResult> GetBusinessImages(int businessId)
        {
            try
            {
                var business = await _context.Businesses
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == businessId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var images = business.Images.Select(i => new
                {
                    i.Id,
                    i.ImageUrl,
                    i.IsPrimary
                });

                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{businessId}/set-primary-image/{imageId}")]
        [Authorize]
        public async Task<IActionResult> SetPrimaryImage(int businessId, int imageId)
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

                var business = await _context.Businesses
                    .Include(b => b.Images)
                    .FirstOrDefaultAsync(b => b.Id == businessId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var image = business.Images.FirstOrDefault(i => i.Id == imageId);
                if (image == null)
                {
                    return NotFound(new { message = "Nie znaleziono pliku" });
                }

                foreach (var img in business.Images)
                {
                    img.IsPrimary = false;
                }

                image.IsPrimary = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Obraz został ustawiony jako obraz główny", imageUrl = image.ImageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpDelete("{businessId}/delete-image/{imageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteBusinessImage(int businessId, int imageId)
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

                var business = await _context.Businesses
                    .Include(b => b.Images)
                    .FirstOrDefaultAsync(b => b.Id == businessId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var image = business.Images.FirstOrDefault(i => i.Id == imageId);
                if (image == null)
                {
                    return NotFound(new { message = "Nie znaleziono pliku" });
                }

                var imageUrl = image.ImageUrl;
                var objectName = imageUrl.Split('/').Last();
                var googleCloudStorageService = new GoogleCloudStorageService();

                await googleCloudStorageService.DeleteFileAsync(objectName);


                _context.BusinessImages.Remove(image);
                await _context.SaveChangesAsync();

                var isPublished = await _businessValidateService.ValidateBusinessIsPublished(businessId);

                if (!isPublished)
                {
                    await _businessValidateService.UnpublishBusinessAsync(businessId);
                    return Ok(new { message = "Zdjęcie zostało usunięte, biznes został niepublikowany" });
                }

                return Ok(new { message = "Zdjęcie profilu zostało usunięte" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{businessId}/toggle-publish")]
        [Authorize]
        public async Task<IActionResult> TogglePublishBusiness(int businessId)
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

                var business = await _context.Businesses.FindAsync(businessId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                if (business.IsPublished)
                {
                    business.IsPublished = false;
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Biznes został niepublikowany" });
                }
                else
                {
                    var canBePublished = await _businessValidateService.ValidateBusinessIsPublished(businessId);

                    if (!canBePublished)
                    {
                        return BadRequest(new { message = "Biznes nie spełnia wymagań do publikacji" });
                    }

                    business.IsPublished = true;

                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Biznes został opublikowany" });
                }  
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPost("{businessId}/reviews")]
        [Authorize]
        public async Task<IActionResult> AddReview(int businessId, [FromBody] AddReviewDto reviewDto)
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

                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var review = _mapper.Map<Review>(reviewDto);
                review.UserId = userId.Value;
                review.BusinessId = businessId;
                review.CreatedAt = DateTime.UtcNow;

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                await UpdateAverageRating(businessId);
                await UpdateReviewCount(businessId);

                return Ok(new { message = "Twoja opinia została dodana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}/reviews")]
        public async Task<IActionResult> GetBusinessReviews(int businessId)
        {
            try
            {
                var business = await _context.Businesses.FindAsync(businessId);

                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var reviews = _context.Reviews.Where(r => r.BusinessId == businessId).Include(r => r.User).ToList();

                var reviewDtos = _mapper.Map<List<BusinessReviewDto>>(reviews);

                return Ok(reviewDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}/reviews/exists")]
        [Authorize]
        public async Task<IActionResult> CheckUserReview(int businessId)
        {
            try
            {

                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }

                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var hasReviewed = await _context.Reviews.AnyAsync(r => r.BusinessId == businessId && r.UserId == userId.Value);

                return Ok(hasReviewed);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{businessId}/reviews/has-access")]
        [Authorize]
        public async Task<IActionResult> CheckAccessToReview(int businessId)
        {
            try
            {
                var userId = _userService.GetUserId(User);
                if (userId == null)
                {
                    return Unauthorized(new { message = "Użytkownik nieautoryzowany" });
                }
                var business = await _context.Businesses.FindAsync(businessId);
                if (business == null)
                {
                    return NotFound(new { message = "Nie znaleziono biznesu" });
                }

                var hasAccessToReview = await _context.Bookings
                    .Include(b => b.Service)
                    .Where(b => b.UserId == userId.Value && b.Service.BusinessId == businessId && b.Status == "completed")
                    .AnyAsync();

                return Ok(hasAccessToReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        private async Task UpdateAverageRating(int businessId)
        {
            var business = await _context.Businesses.Include(b => b.Reviews).FirstOrDefaultAsync(b => b.Id == businessId);
            
            if (business != null)
            {
                business.AverageRating = business.Reviews.Any() ? business.Reviews.Average(r => r.Rating) : 0;
                await _context.SaveChangesAsync();
            }
        }

        private async Task UpdateReviewCount(int businessId)
        {
            var business = await _context.Businesses.Include(b => b.Reviews).FirstOrDefaultAsync(b => b.Id == businessId);

            if (business != null)
            {
                business.ReviewCount = business.Reviews.Count;
                await _context.SaveChangesAsync();
            }
        }
    }
}
