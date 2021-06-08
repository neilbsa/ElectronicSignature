using ElectronicSignature.Core.Repository.Core.Implementations;
using ElectronicSignature.Data;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Implementation
{
    public class FileRepositoryService : Repository<FileRepository>, IFileRepository
    {

        public ApplicationDbContext _context { get; set; }
        public IWebHostEnvironment _env { get; set; }
        public IHttpContextAccessor _httpAccessor { get; set; }

        public FileRepositoryService(ApplicationDbContext cont, IWebHostEnvironment env, IHttpContextAccessor access) 
       :base(cont)
        {
            _context = cont;
            _env = env;
            _httpAccessor = access;
        }


        public async Task UploadFromDocument(Document doc)
        {
            if (doc != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "Files", "PDF");
                var filePath = Path.Combine(uploads, String.Format("{0}.{1}", doc.Id.ToString(), "pdf"));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await doc.uploadedFile.CopyToAsync(fileStream);
                }

                if(doc.SupportingDocsForUpload != null && doc.SupportingDocsForUpload.Count() > 0)
                {
                    var SupportingDocBasepath = Path.Combine(_env.WebRootPath, "Files", "SupportingDocuments",doc.Id.ToString());
                    if (!Directory.Exists(SupportingDocBasepath))
                    {
                        Directory.CreateDirectory(SupportingDocBasepath);
                    }

                    foreach(var item in doc.SupportingDocsForUpload)
                    {
                        var SupportingDocPath = Path.Combine(SupportingDocBasepath, String.Format("{0}", item.FileName ));
                        using (var fileStream = new FileStream(SupportingDocPath, FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                    }              
                }

            }
        }

        public async Task<FileRepository> GetFileRepositoryPath(Guid Id)
        {
            var currentItem = await GetDetailsAsync(Id);

            if(currentItem == null)
            {
                throw new Exception("File not Found");
            }
            else
            {
                var SupportingDocBasepath = Path.Combine(_env.WebRootPath, "Files", "SupportingDocuments", currentItem.DocumentId.ToString());
               var SupportingDocPath = Path.Combine(SupportingDocBasepath, String.Format("{0}", currentItem.Filename));

                string vpath = String.Format("~/Files/SupportingDocuments/{0}/{1}", currentItem.DocumentId.ToString(), currentItem.Filename);
                currentItem.VirtualPath = vpath;
                currentItem.FullPath = SupportingDocPath;
                return currentItem;
            }





        }
    }
}
