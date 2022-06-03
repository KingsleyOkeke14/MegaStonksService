﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
