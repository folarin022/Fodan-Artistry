using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.DTOs
{
    public class VerifyEmailDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
