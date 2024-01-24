using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibleVerse.DTO.Models
{
    public class UserSignUpRequest
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Email { get; set; }

        public string RefCode { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        

    }
}
