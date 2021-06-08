using ElectronicSignature.Core.Repository.Core.Implementations;
using ElectronicSignature.Data;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.Models;
using ElectronicSignature.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;   

namespace ElectronicSignature.DataRepositories.Implementation
{
    public class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        public ApplicationDbContext _context { get; set; }
        public IWebHostEnvironment _env { get; set; }
        public IHttpContextAccessor _httpAccessor { get; set; }
        public IFileRepository _fileRepo { get; set; }
        public IEmailSender _emailSender { get; set; }
        public IFolderStructureService _folderStructure { get; set; }
        public IDocumentFolderLink _DocFolLink { get; set; }
        public DocumentRepository(
            ApplicationDbContext cont,
            IWebHostEnvironment env,
            IHttpContextAccessor access, 
            IEmailSender sender,
            IFileRepository fileRepo, 
            IFolderStructureService folder,IDocumentFolderLink DocFolLink
          ) : base(cont)
        {
            _context = cont;
            _env = env;
            _emailSender = sender;
            _httpAccessor = access;
            _fileRepo = fileRepo;
            _folderStructure = folder;
            _DocFolLink = DocFolLink;


        }


       



        private string emailBody = @"           
            <h2 style=""color: #0838b9;"">Good Day</h2>
            <p>This is Electronic Signature from Civic Merchandising INC.</p>
            <p>You recieve a for signature document from {0}</p>
            <p>You can click &nbsp; <a href=""https://esignature.civicmdsg.com.ph:11443/Home/UpdateSignatures/{1}"">HERE</a>&nbsp; to redirect to the page</p>
            <table style=""border-size=1px;border-style:solid;border-color:black;"">
            <thead>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > Detail </ td >
            <td style=""width: 252.8px; border: 1px solid #dddddd;""></td>
            </tr>
            </thead>
            <tbody>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > DocumentId </ td >
            <td style=""width: 252.8px;border: 1px solid #dddddd;"">{2}</td>
            </tr>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > Filename </ td >
            <td style=""width: 252.8px; border: 1px solid #dddddd;"">{3}</td>
            </tr>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > Remarks </ td >
            <td style=""width: 252.8px; border: 1px solid #dddddd;"">{4}</td>
            </tr>
            </tbody>
            </table>
            <p><strong>&nbsp;</strong></p>
            <p><strong>Best Regards,</strong></p>
            <p>&nbsp;</p>           
            ";





        public override async Task<bool> DeleteEntity(Guid Id)
        {
            var detail = await GetDetailsAsync(Id);
            var deleteFolderId = await _folderStructure.GetDeleteFolderId(getUserEmail());
            bool returnVal = false;
            try
            {
               if(detail != null)
                {
                   returnVal = true;
                   await TransferDocumentToFolder(deleteFolderId.Id,Id);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting:" + e.Message);
            }



            return returnVal;
        }




        public override async Task<Document> AddEntityAsync(Document entity)
        {
            List<FileRepository> repo = new List<FileRepository>();

            entity.Filename = entity.uploadedFile.FileName;
            entity.Status = "Open";

         

            if (entity.IsScheduledDelivery)
            {
                foreach (var signa in entity.AssignedSignatories)
                {
                    signa.Status = "Hidden";
                }
            }
            else
            {
                foreach (var signa in entity.AssignedSignatories)
                {
                    signa.Status = "Pending";
                }
            }





            if (entity.SupportingDocsForUpload != null && entity.SupportingDocsForUpload.Count() > 0)
            {
                foreach (var item in entity.SupportingDocsForUpload)
                {
                    repo.Add(new FileRepository() { ContentSize = item.Length, ContentType = item.ContentType, Filename = item.FileName });
                }

            }
            entity.SupportingDocuments = repo;
            var d = await base.AddEntityAsync(entity);
            await _fileRepo.UploadFromDocument(d);


            var ownerdefaultFolder = await _folderStructure.GetDefaultFolderId(entity.CreateUser);
            await _DocFolLink.AddFileToFolder(ownerdefaultFolder, d);


            foreach (var item in entity.AssignedSignatories) {

                var defaultFolder = await _folderStructure.GetDefaultFolderId(item.EmailAddress);
                await _DocFolLink.AddFileToFolder(defaultFolder, d);
            }      
           return d;
        }

        public async Task SendNotificationEmail(Guid Id)
        {

            var model = await GetDetailsAsync(Id);

            if (model != null)
            {
                //construct here
                EmailSenderModel mod = new EmailSenderModel();

                mod.Subject = "Electronic Signature: Notification";
                mod.IsHtml = true;
                mod.Body = String.Format(emailBody, model.CreateUser, model.Id, model.Id, model.Filename, model.Remarks);


                if (model.IsScheduledDelivery)
                {

                    //make schedule
                    var first = GetCurrentOrderNumber(model);

                    var forSending = model.AssignedSignatories
                        .Where(x => x.Status == "Hidden" && x.OrderNumber == first)
                        .ToList();

                    mod.TO = forSending.Select(x => x.EmailAddress.Trim()).ToList();

                    foreach(var item in forSending)
                    {
                        item.Status = "Pending";
                    }
                }
                else
                {

                    var forSending = model.AssignedSignatories
                        .Where(x => x.Status == "Hidden")
                        .ToList();
                    foreach (var item in forSending)
                    {
                        item.Status = "Pending";
                    }
                    //send all the same time
                    mod.TO = forSending.Select(x => x.EmailAddress.Trim()).ToList();
                }




                model.Status = "Sent";
                try
                {
                    await _emailSender.SendEmailAsync(mod);
               
                }
                catch (Exception e)
                {

                    throw new Exception("Sending-Error: " + e.Message);
                  
                }

               await UpdateEntityAsync(model);
            }
        }
        private string getUserEmail()
        {

            return _httpAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Email).Value;
        }

        private string emailBodyNotifReply = @"           
            <h2 style=""color: #0838b9;"">Good Day {0}</h2>
            <p>This is Electronic Signature from Civic Merchandising INC.</p>
            <p>We would like to inform you that file {2} is ready for extraction</p>            
            <table style=""border-size=1px;border-style:solid;border-color:black;"">
            <thead>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > Detail </ td >
            <td style=""width: 252.8px; border: 1px solid #dddddd;""></td>
            </tr>
            </thead>
            <tbody>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > DocumentId </ td >
            <td style=""width: 252.8px;border: 1px solid #dddddd;"">{1}</td>
            </tr>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > Filename </ td >
            <td style=""width: 252.8px; border: 1px solid #dddddd;"">{2}</td>
            </tr>
            <tr>
            <td style = ""width: 189.6px; border: 1px solid #dddddd;"" > Remarks </ td >
            <td style=""width: 252.8px; border: 1px solid #dddddd;"">{3}</td>
            </tr>
            </tbody>
            </table>
            <p><strong>&nbsp;</strong></p>
            <p>You can click &nbsp; <a href=""https://esignature.civicmdsg.com.ph:11443/Home/Details/{1}"">HERE</a>&nbsp; to redirect to the detail page</p>
            <p><strong>Best Regards,</strong></p>
            <p>&nbsp;</p>           
            ";
        private int GetCurrentOrderNumber(Document doc)
        {
    
            var currentOrderNumber = doc.AssignedSignatories
            .Where(x => x.Status == "Hidden")
            .Select(x => x.OrderNumber)
            .Distinct()
            .OrderBy(x => x)
            .ToList().FirstOrDefault();

            return currentOrderNumber;
        }
        private async Task SendDocumentReadyForExtractionEmail(Guid Id)
        {
            var doc = await GetDetailsAsync(Id);
            if (doc != null)
            {

                //construct here
                EmailSenderModel mod = new EmailSenderModel();

                mod.Subject = "Electronic Signature: Notification";
                mod.IsHtml = true;
                mod.Body = String.Format(emailBodyNotifReply, doc.CreateUser, doc.Id, doc.Filename, doc.Remarks);
                mod.TO = new List<string>() { doc.CreateUser };

                //var doc = await GetDetailsAsync(Id);

                try
                {
                    doc.Status = "Signed";
                    await _emailSender.SendEmailAsync(mod);
                    await UpdateEntityAsync(doc);
                }
                catch (Exception e)
                {

                    throw new Exception("Sending-Error: " + e.Message);

                }
            }

        }

        public async Task UpdateSignatories(Guid Id)
        {
            var currentUser = getUserEmail();
            var doc = await GetDetailsAsync(Id);
            var currentSignatory = doc.AssignedSignatories.Where(x => x.EmailAddress.ToLower() == currentUser.ToLower()).ToList();
            List<Signatories> forUpdateSignatories = new List<Signatories>();      


            if (doc.IsScheduledDelivery)
            {
                var currentOrderNumber =  GetCurrentOrderNumber(doc);            
            }


            foreach (var item in forUpdateSignatories)
            {
                item.Status = "Signed";
            }
            await UpdateEntityAsync(doc);

            if (!doc.AssignedSignatories.Any(x => x.Status =="Pending"))
            {
                try
                {
                    await SendDocumentReadyForExtractionEmail(Id);
                }catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        public async Task AddNewSignatoryToDocument(Signatories mod)
        {
            var model = await GetDetailsAsync(mod.DocumentId);

            if(model != null)
            {

                EmailSenderModel emailModel = new EmailSenderModel();
                emailModel.Subject = "Electronic Signature: Notification";
                emailModel.IsHtml = true;
                emailModel.Body = String.Format(emailBody, model.CreateUser, model.Id, model.Id, model.Filename, model.Remarks);
                emailModel.TO = new List<string>() { mod.EmailAddress };


                try
                {
                    await _emailSender.SendEmailAsync(emailModel);

                }
                catch (Exception e)
                {

                    throw new Exception("Sending-Error: " + e.Message);

                }
            }
        }
      
        public List<Document> GetAllFilesInFolder(Guid Id)
        {
            return GetList(x => x.FolderIncluded.Any(x=>x.FolderStrucId == Id) && x.IsDeleted == false).ToList();
        }

        public async Task TransferDocumentToFolder(Guid folderId, Guid fileId)
        {

            var user = getUserEmail();
            var fileDetails = await GetDetailsAsync(fileId);
            var folderStruc = fileDetails.FolderIncluded.Where(x => x.FolderStrucDetail.OwnerUserEmail == user).FirstOrDefault();
            if(folderStruc != null)
            {
                folderStruc.FolderStrucId = folderId;
                await _DocFolLink.UpdateEntityAsync(folderStruc);
            }
            else
            {
                throw new Exception("Folder cannot be found");
            }





        }
    }
}
