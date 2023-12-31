﻿using System.ComponentModel.DataAnnotations;

namespace MyTasks_WebAPI.Models.Authentication
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "UserRole is required")]
        public string UserRole { get; set; }
    }
}
