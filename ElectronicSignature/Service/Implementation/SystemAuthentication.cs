using ElectronicSignature.Models;
using ElectronicSignature.Service.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static ElectronicSignature.Areas.Identity.Pages.Account.LoginModel;

namespace ElectronicSignature.Service.Implementation
{
    public class SystemAuthentication : ISystemAuthentication
    {

        private IConfiguration Configuration { get; set; }
        public SystemAuthentication(IConfiguration conf)
        {
            Configuration = conf;
        }


        public async Task<TokenModel> LogOn(InputModel mod)
        {
            TokenModel models = new TokenModel();



            string securityHostName = Configuration["SecurityServer"];
            string AuthenticationServer = securityHostName;



            string uri = AuthenticationServer;
            string currentContent = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.ExpectContinue = false;
                // client.DefaultRequestHeaders.Add("Authorization", token);

                IEnumerable<KeyValuePair<string, string>> AuthParams = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("username",mod.UserName),
                    new KeyValuePair<string, string>("password",mod.Password)
                };

                HttpContent PostContent = new FormUrlEncodedContent(AuthParams);
                PostContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                using (HttpResponseMessage response = await client.PostAsync(uri, PostContent))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (HttpContent content = response.Content)
                        {
                            currentContent = await content.ReadAsStringAsync();
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            return JsonConvert.DeserializeObject<TokenModel>(currentContent);
        }

        public Task<AuthModel> ValidateModel(TokenModel mod)
        {


            //No use if token is empty
            if (String.IsNullOrEmpty(mod.access_token))
                return Task.FromResult<AuthModel>(null);

            //Logout first
            //await LogoutAsync();
            var issuer = Configuration["Issuer"];
            var audience = Configuration["Audience"];
            var sec = Configuration["Secret"];
            bool ValidateIssuer = Boolean.Parse(Configuration["ValidateIssuer"]);
            bool ValidateAudience = Boolean.Parse(Configuration["ValidateAudience"]);
            bool ValidateLifeTime = Boolean.Parse(Configuration["ValidateLifeTime"]);
            bool ValidateIssuerSigningKey = Boolean.Parse(Configuration["ValidateIssuerSigningKey"]);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenParam = new TokenValidationParameters()
            {
                ValidateIssuer = ValidateIssuer,
                ValidateAudience = ValidateAudience,
                ValidateLifetime = ValidateLifeTime,
                ValidateIssuerSigningKey = ValidateIssuerSigningKey,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(sec))
            };



            // Retrieve principal from Jwt token
            var principal = tokenHandler.ValidateToken(mod.access_token, tokenParam, out var validatedToken);

            // Cast needed for accessing claims property
            var identity = principal.Identity as ClaimsIdentity;

            // parse jwt token to get all claims
            var securityToken = tokenHandler.ReadToken(mod.access_token) as JwtSecurityToken;

            // Search for missed claims, for example claim 'sub'
            var extraClaims = securityToken.Claims.Where(c => !identity.Claims.Any(x => x.Type == c.Type)).ToList();

            // Adding the original Jwt has 2 benefits:
            //  1) Authenticate REST service calls with orginal Jwt
            //  2) The original Jwt is available for renewing during sliding expiration
            extraClaims.Add(new Claim("jwt", mod.access_token));

            // Merge claims
            identity.AddClaims(extraClaims);


            // Setup authenticaties 
            // ExpiresUtc is used in sliding expiration

            var timeIssue = new DateTimeOffset(DateTime.Parse(identity.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Iat).First().Value));
            var expiresIn = new DateTimeOffset(DateTime.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Expiration).First().Value));
            var authenticationProperties = new AuthenticationProperties()
            {
                IssuedUtc = timeIssue,
                ExpiresUtc = expiresIn,
                IsPersistent = false
            };

            AuthModel Authmod = new AuthModel()
            {
                IsSuccess = true,
                AuthProperties = authenticationProperties,
                Principal = principal
            };


            // The actual Login
            //await httpContext.Authentication.SignInAsync(authenticationSettings.AuthenticationScheme, principal, authenticationProperties);

            return Task.FromResult(Authmod);

        }


    }
}
