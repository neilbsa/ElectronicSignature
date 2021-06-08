using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.Service.Interface
{
    public interface IEmailSender
    {

        Task SendEmailAsync(EmailSenderModel mod);

    }
}
