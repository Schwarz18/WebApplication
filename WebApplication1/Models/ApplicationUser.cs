﻿using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? Address { get; set; }
    }
}
