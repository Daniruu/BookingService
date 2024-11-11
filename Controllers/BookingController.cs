using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly BookingServiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public BookingController(BookingServiceDbContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _userService.GetUserId(User);

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Nie znaleziono użytkownika" });
                }

                var service = await _context.Services
                    .Include(s => s.Employee)
                    .FirstOrDefaultAsync(s => s.Id == bookingCreateDto.ServiceId);

                if (service == null)
                {
                    return NotFound(new { message = "Nie znaleziono usługi" });
                }

                var employee = service.Employee;
                if (employee == null)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika" });
                }

                var utcDateTime = bookingCreateDto.DateTime.ToUniversalTime();
                
                var conflictingBooking = await _context.Bookings
                    .Include(b => b.Service)
                    .Where(b => b.Service.EmployeeId == employee.Id && b.Status == "pending")
                    .FirstOrDefaultAsync(b =>
                        (b.DateTime <= utcDateTime && utcDateTime < b.DateTime.AddMinutes(b.Service.Duration)) ||
                        (utcDateTime <= b.DateTime && b.DateTime < utcDateTime.AddMinutes(service.Duration)));

                if (conflictingBooking != null)
                {
                    return BadRequest(new { message = "Ten czas został już zarezerwowany" });
                }

                var booking = _mapper.Map<Booking>(bookingCreateDto);
                booking.UserId = user.Id;
                booking.CreatedAt = DateTimeOffset.UtcNow;
                booking.Status = "pending";
                booking.DateTime = utcDateTime;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var bookingDetailDto = _mapper.Map<BookingDetailDto>(booking);
                return Ok(new { message = "Usługa została zarezerwowana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingDetail(int id)
        {
            try
            {
                var userId = _userService.GetUserId(User);

                var booking = await _context.Bookings
                    .Include(b => b.Service)
                        .ThenInclude(s => s.Employee)
                    .Include(b => b.Service)
                        .ThenInclude(s => s.Business)
                            .ThenInclude(b => b.Images)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound(new { message = "Nie znaleziono rezerwacji" });
                }

                if (booking.UserId != userId && booking.Service.Business.UserId != userId)
                {
                    return Forbid();
                }

                var bookingDetailDto = _mapper.Map<BookingDetailDto>(booking);
                return Ok(bookingDetailDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, BookingUpdateDto bookingUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _userService.GetUserId(User);

                var booking = await _context.Bookings
                    .Include(b => b.Service)
                    .ThenInclude(s => s.Business)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound(new { message = "Nie znaleziono rezerwacji" });
                }

                if (booking.UserId != userId && booking.Service.Business.UserId != userId)
                {
                    return Forbid();
                }

                var service = await _context.Services
                    .Include(s => s.Employee)
                    .FirstOrDefaultAsync(s => s.Id == booking.ServiceId);

                if (service == null)
                {
                    return NotFound(new { message = "Nie znaleziono usługi" });
                }

                var employee = service.Employee;
                if (employee == null)
                {
                    return NotFound(new { message = "Nie znaleziono pracownika" });
                }

                var utcDateTime = bookingUpdateDto.DateTime.ToUniversalTime();

                var conflictingBooking = await _context.Bookings
                    .Include(b => b.Service)
                    .Where(b => b.Service.EmployeeId == employee.Id && b.Status == "pending")
                    .FirstOrDefaultAsync(b =>
                        (b.DateTime <= utcDateTime && utcDateTime < b.DateTime.AddMinutes(b.Service.Duration)) ||
                        (utcDateTime <= b.DateTime && b.DateTime < utcDateTime.AddMinutes(service.Duration)));

                if (conflictingBooking != null)
                {
                    return BadRequest(new { message = "Ten czas został już zarezerwowany" });
                }

                _mapper.Map(bookingUpdateDto, booking);
                booking.DateTime = utcDateTime;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Rezerwacja zaktualizowana" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Błąd bazy danych", details = ex.InnerException?.Message ?? ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var userId = _userService.GetUserId(User);

                var booking = await _context.Bookings
                    .Include(b => b.Service)
                    .ThenInclude(s => s.Business)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound(new { message = "Nie znaleziono rezerwacji" });
                }

                if (booking.UserId != userId && booking.Service.Business.UserId != userId)
                {
                    return Forbid();
                }

                booking.Status = "cancelled";
                _context.Bookings.Update(booking);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Rezerwacja została anulowana" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }

        [HttpGet("available-slots")]
        [Authorize]
        public async Task<IActionResult> GetAvailableSlots(int serviceId, DateTimeOffset dateTime)
        {
            try
            {
                var service = await _context.Services
                    .Include(s => s.Employee)
                    .ThenInclude(e => e.WorkingHours)
                    .Include(s => s.Business)
                    .ThenInclude(b => b.WorkingHours)
                    .FirstOrDefaultAsync(s => s.Id == serviceId);

                if (service == null)
                {
                    return NotFound(new { message = "Nie znaleziono usługi" });
                }

                var employee = service.Employee;
                var business = service.Business;

                var dayOfWeek = dateTime.DayOfWeek.ToString();
                var businessWorkingHours = business.WorkingHours.FirstOrDefault(wh => wh.DayOfWeek == dayOfWeek);

                if (businessWorkingHours == null)
                {
                    return BadRequest(new { message = "Firma nie pracuje w wybranym dniu" });
                }

                var workingStart = businessWorkingHours.Start;
                var workingEnd = businessWorkingHours.End;

                var exitingBookings = await _context.Bookings.Where(b => b.Service.EmployeeId == employee.Id && b.DateTime.Date == dateTime.Date && b.Status == "pending").Include(b => b.Service).ToListAsync();

                var serviceDuration = TimeSpan.FromMinutes(service.Duration);
                var interval = TimeSpan.FromMinutes(15);

                var availableSlots = new List<DateTimeOffset>();
                var currentTimeSlot = dateTime.Date.Add(workingStart);

                while (currentTimeSlot.Add(serviceDuration) <= dateTime.Date.Add(workingEnd))
                {
                    var isSlotAvailable = !exitingBookings.Any(b =>
                        (
                            (b.DateTime <= currentTimeSlot && currentTimeSlot < b.DateTime.AddMinutes(b.Service.Duration)) ||
                            (currentTimeSlot <= b.DateTime && b.DateTime < currentTimeSlot.Add(serviceDuration))
                        )
                    );

                    if (isSlotAvailable)
                    {
                        availableSlots.Add(currentTimeSlot);
                    }

                    currentTimeSlot = currentTimeSlot.Add(interval);
                }

                return Ok(availableSlots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", details = ex.Message });
            }
        }
    }
}
