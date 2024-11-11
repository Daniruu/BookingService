﻿using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BookingUpdateDto
    {
        [Required]
        public DateTimeOffset DateTime { get; set; }
    }
}
