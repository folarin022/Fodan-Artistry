using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.DTOs.AccountModel
{
    public class RegisterDto
    {
        public string FIrstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
    }
}
