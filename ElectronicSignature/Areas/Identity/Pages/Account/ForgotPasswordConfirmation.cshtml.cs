﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElectronicSignature.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("AccessDenied");
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("AccessDenied");
        }
    }
}
