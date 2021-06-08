using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ElectronicSignature.Areas.Identity.Pages.Account.LoginModel;

namespace ElectronicSignature.Service.Interface
{
    public interface ISystemAuthentication
    {
        Task<TokenModel> LogOn(InputModel mod);
        Task<AuthModel> ValidateModel(TokenModel mod);
    }
}
