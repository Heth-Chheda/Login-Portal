﻿using System;
namespace LoginPortal.Models
{
    public class ResetPasswordRequest
    {

                public string Email { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
    }
}



