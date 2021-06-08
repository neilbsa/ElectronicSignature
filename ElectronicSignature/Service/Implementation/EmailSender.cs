
using ElectronicSignature.Models;
using ElectronicSignature.Service.Interface;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicSignature.Service.Implementation
{
    public class EmailSender : IEmailSender
    {


        private readonly IHttpContextAccessor _access;
        public EmailSender(IHttpContextAccessor access)
        {
            _access = access;
        }


        private string getUserToken()
        {
            return _access.HttpContext.User.FindFirst(x => x.Type == "jwt").Value;
        }
        public async Task SendEmailAsync(EmailSenderModel mod)
        {

            string myParam = JsonConvert.SerializeObject(mod);
            string currentContent = string.Empty;
            string token = "bearer" + " " + getUserToken();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.ExpectContinue = false;
                client.DefaultRequestHeaders.Add("Authorization", token);

                var form = new MultipartFormDataContent();

                if(mod.TO != null)
                {
                    var currentTO = JsonConvert.SerializeObject(mod.TO.ToArray());
                    form.Add(new StringContent(currentTO, Encoding.UTF8, "application/json"), "TO");
                }


                if (mod.BCC != null)
                {
                    var currentBCC = JsonConvert.SerializeObject(mod.BCC.ToArray());
                    form.Add(new StringContent(currentBCC, Encoding.UTF8, "application/json"), "BCC");

                }

                if (mod.CC != null)
                {
                    var currentCC = JsonConvert.SerializeObject(mod.CC.ToArray());
                    form.Add(new StringContent(currentCC, Encoding.UTF8, "application/json"), "CC");
                }


                form.Add(new StringContent(mod.Body), "Body");
                form.Add(new StringContent(mod.Subject), "Subject");
                form.Add(new StringContent(mod.IsHtml.ToString()), "IsHtml");
                var emailConfig = JsonConvert.SerializeObject(mod.EmailConfiguration);
                form.Add(new StringContent(emailConfig, Encoding.UTF8, "application/json"), "EmailConfiguration");

                using (HttpResponseMessage response = await client.PostAsync("http://localhost:44352/api/EmailProcessor/SendEmail", form))
                //using (HttpResponseMessage response = await client.PostAsync("https://civic.civicmdsg.com.ph:42515/api/EmailProcessor/SendEmail", form))
                {
                    using (HttpContent content = response.Content)
                    {
                        currentContent = await content.ReadAsStringAsync();
                    }
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var cur = JsonConvert.DeserializeObject<SendingEmailResponseModel>(currentContent);
                        throw new Exception(cur.ErrorMessage);
                    }
                }

            }
        }
    }
}
