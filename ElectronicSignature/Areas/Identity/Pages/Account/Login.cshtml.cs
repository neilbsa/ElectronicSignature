using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ElectronicSignature.Service.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using ElectronicSignature.DataRepositories.Interface;

namespace ElectronicSignature.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ISystemAuthentication _authServe;
        private readonly IHttpContextAccessor _access;
        private readonly IFolderStructureService _folderStructure;

        public LoginModel(ISystemAuthentication auth, 
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager, IHttpContextAccessor access, IFolderStructureService folderStruct)
        {
            _authServe = auth;
            _access = access;
              _userManager = userManager;       
            _logger = logger;
            _folderStructure = folderStruct;

        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]       
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _authServe.LogOn(Input);
                if (result != null)
                {
                    //_logger.LogInformation("User logged in.");
                    var validateToken = await _authServe.ValidateModel(result);
                    if (validateToken.IsSuccess)
                    {
         
                        // await HttpContext.Authentication.SignInAsync(JwtBearerDefaults.AuthenticationScheme, validateToken.Principal, validateToken.AuthProperties);
                        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(
                           _access.HttpContext, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                            validateToken.Principal,
                            validateToken.AuthProperties);
                 
                    }


             
                    return LocalRedirect(returnUrl);

                }



                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
