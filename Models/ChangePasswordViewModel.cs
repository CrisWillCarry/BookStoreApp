﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BookStoreApp.Models
{
    public class ChangePasswordViewModel
    {
        public string Username { get; set; } 

        [Required(ErrorMessage = "Please enter your password.")]
        public string OldPassword { get; set; } 

        [Required(ErrorMessage = "Please enter your new password.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string NewPassword { get; set; } 

        [Required(ErrorMessage =
            "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } 
    }

}
