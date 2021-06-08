using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicSignature.Models
{
    public class AuthModel
    {
        public bool IsSuccess { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public AuthenticationProperties AuthProperties { get; set; }
    }
}
