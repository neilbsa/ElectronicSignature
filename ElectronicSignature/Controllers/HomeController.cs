using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ElectronicSignature.Models;
using ElectronicSignature.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using iTextSharp.text.pdf;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.Core.Repository.Core.Interfaces;

namespace ElectronicSignature.Controllers
{


    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ApplicationDbContext _context { get; set; }
        private readonly IHttpContextAccessor _access;
        public IWebHostEnvironment _env { get; set; }
        public IDocumentRepository _documentRepo { get; set; }
        public IElementCoordinates _elementCoor { get; set; }
        public IFileRepository _fileRepository { get; set; }
        public ISignatories _signa { get; set; }
        public IFolderStructureService _folderStruc { get; set; }
        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext cont,
            IWebHostEnvironment env,
            IHttpContextAccessor access,
            IDocumentRepository documentRepo,
            IElementCoordinates elementCoor,
            ISignatories signa,
            IFileRepository fileRepo,
            IFolderStructureService folderStruc
            )
        {
            _folderStruc = folderStruc;
            _access = access;
            _env = env;
            _signa = signa;
            _logger = logger;
            _context = cont;
            _documentRepo = documentRepo;
            _elementCoor = elementCoor;
            _fileRepository = fileRepo;

        }


        public IActionResult GetAllFolders()
        {
            try
            {
                var folders = _folderStruc.GetList(x => x.OwnerUserEmail == getUserEmail() && x.MotherFolderId == null);
                if (folders.Count() <= 0)
                {
                    _folderStruc.CreateInitialFolder(getUserEmail());
                }
                folders = _folderStruc.GetList(x => x.OwnerUserEmail == getUserEmail() && x.MotherFolderId == null);
                return Ok(folders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFileToFolder(Guid fileId)
        {
            try
            {
                var result = await _documentRepo.DeleteEntity(fileId);

                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("error Deleting contact administrator");
                }
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> TransferFileToFolder(Guid folderId, Guid fileId)
        {
            try
            {
               await _documentRepo.TransferDocumentToFolder(folderId, fileId);
               return Ok();
            }
            catch(Exception e)
            {
               return BadRequest(e.Message);
            }        
        }



        public IActionResult GetAllMovableFolders()
        {
            try
            {
                var folders = _folderStruc.GetList(x => x.OwnerUserEmail == getUserEmail() && x.MotherFolderId == null && !x.text.Contains("Deleted"));

                return Ok(folders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }





        string[] filterSetOpt = new string[] { "All", "Waiting for others", "Waiting for me", "Signed", "Draft" };
        string[] sortOption = new string[] { "Document name", "Create Date" };
        private async Task<List<Document>> getDocumentWithSignatureUpdate()
        {


            string currentuser = getUserEmail();



            return (from d in await _documentRepo.GetAllAsync()
                    where d.CreateUser == currentuser || d.AssignedSignatories.Any(x => x.EmailAddress == currentuser)

                    select new Document()
                    {


                        Id = d.Id,
                        Filename = d.Filename,
                        CreateUser = d.CreateUser,
                        Status = OwnerUpdate(d),
                        AssignedSignatories = d.AssignedSignatories,
                        SupportingDocuments = d.SupportingDocuments,
                        IsOwned = (d.CreateUser == currentuser),
                        FolderIncluded = d.FolderIncluded

                    }).ToList();


        }

        public string OwnerUpdate(Document doc)
        {
            string currentuser = getUserEmail();

            string status = String.Empty;


            //waiting for me
            if (doc.AssignedSignatories.Any(c => c.EmailAddress == currentuser && c.Status == "Pending"))
            {
                status = "Waiting for me";
            }

            if (!doc.AssignedSignatories.Any(c => c.EmailAddress == currentuser && c.Status == "Pending") && doc.AssignedSignatories.Any(c => c.Status == "Pending"))
            {
                status = "Waiting for others";
            }

            if (!doc.AssignedSignatories.Any(c => c.EmailAddress == currentuser && c.Status == "Pending") && !doc.AssignedSignatories.Any(c => c.Status == "Pending"))
            {
                status = "Signed";
            }

            //waiting for me
            if (doc.Status == "Open")
            {
                status = "Draft";
            }


            return status;

        }


        public async Task<IActionResult> RefillIndex(string filterOpt, string sortBy, Guid selectedFolderId)
        {

            var items = await getDocumentWithSignatureUpdate();

            if (!String.IsNullOrEmpty(filterOpt) || !String.IsNullOrWhiteSpace(filterOpt))
            {
                //all Logic

                items = items.Where(x => filterOpt == "All" || x.Status == filterOpt).ToList();
            }


            var defualtFolder = await _folderStruc.GetDefaultFolderId(getUserEmail());
            //var isDefaultFolder = (selectedFolderId == defualtFolder.Id);
            ////all Logic

            //if (isDefaultFolder)
            //{
            //    items = items.Where(x => x.AssignedSignatories.Any(x => x.EmailAddress == getUserEmail())).ToList();

            //}
            //else
            //{
                items = items.Where(x => x.FolderIncluded.Any(x => x.FolderStrucId == selectedFolderId)).ToList();
            //}





            if (!String.IsNullOrEmpty(sortBy) || !String.IsNullOrWhiteSpace(sortBy))
            {
                //all Logic

                if (sortBy.Equals("Document name"))
                {
                    items = items.OrderBy(x => x.Filename).ToList();
                }
                else if (sortBy.Equals("Create Date"))
                {
                    items = items.OrderBy(x => x.CreateDate).ToList();
                }
            }

            ViewBag.currentUser = getUserEmail();

            return View("IndexItemView", items);
        }


        public async Task<IActionResult> Index()
        {

            await _folderStruc.CreateInitialFolder(getUserEmail());

            var items = await getDocumentWithSignatureUpdate();

            ViewBag.FilterOpt = filterSetOpt;
            ViewBag.sortOption = sortOption;
            ViewBag.currentUser = getUserEmail();
            return View(items.OrderBy(x => x.Filename).ToList());
        }


        public async Task<IActionResult> UpdateSignatures(Guid Id)
        {
            var list = await getDocumentWithSignatureUpdate();
            var doc = list.Where(x => x.Id == Id).FirstOrDefault();

            if (doc != null)
            {

                if (doc.Status != "Signed" && doc.AssignedSignatories.Any(x => x.Status == "Pending" && x.EmailAddress == getUserEmail()))
                {
                    if (doc.AssignedSignatories.Any(x => x.EmailAddress == getUserEmail() && x.Status == "Pending") || doc.CreateUser == getUserEmail())
                    {
                        var signatoryDetails = doc.AssignedSignatories.Where(x => x.EmailAddress == getUserEmail()).FirstOrDefault();

                        ViewBag.SignatoryId = signatoryDetails.Id;
                        ViewBag.SignatoryName = String.Format("{0}<{1}>", signatoryDetails.Name, signatoryDetails.EmailAddress);
                        ViewBag.DocumentId = String.Format("{0}.{1}", doc.Id, "pdf");
                        ViewBag.SignatureTemplates = GetAllSignatureTemplate();
                        return View(doc);
                    }
                    else
                    {
                        return RedirectToAction("Details", new { Id = Id });
                    }
                }
                else
                {
                    return RedirectToAction("Details", new { Id = Id });
                }

            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        private string getUserToken()
        {
            return _access.HttpContext.User.FindFirst(x => x.Type == "jwt").Value;
        }
        private string getUserEmail()
        {

            return _access.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Email).Value;
        }






        private List<string> GetAllSignatureTemplate()
        {
            string email = getUserEmail();
            var templates = _elementCoor.GetAllSignatureTemplate(email);

            return templates.Select(x => x.base64signature).ToList();
        }


        public async Task<IActionResult> AddNewSignatory(Guid Id, string EmailAddress, string Name)
        {

            var model = await _documentRepo.GetDetailsAsync(Id);

            if (model != null)
            {


                try
                {
                    Signatories mod = new Signatories() { DocumentId = Id, EmailAddress = EmailAddress, Name = Name, Status = "Pending" };
                    var d = await _signa.AddEntityAsync(mod);
                    await _signa.SaveAsync();

                    await _documentRepo.AddNewSignatoryToDocument(d);
                    return Json(new { success = true });
                }
                catch (Exception error)
                {
                    return BadRequest(error);
                }

            }



            return null;
        }

        public async Task<IActionResult> SendEmailNotification(Guid Id)
        {
            try
            {
                await _documentRepo.SendNotificationEmail(Id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSignatoryStatus(Guid Id)
        {
            try
            {
                await _documentRepo.UpdateSignatories(Id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Json(new { success = true });
        }




        public IActionResult Create()
        {
            return View();
        }



        private string validateNewFolder(Guid motherId, string text)
        {
            var message = String.Empty;
            if (_folderStruc.GetList(x => x.MotherFolderId == motherId && x.text == text).Any())
            {
                message = "Folder name already Exist";
            }


            return message;

        }


        private string validateUpdateFolder(Guid motherId, string text)
        {
            var message = String.Empty;
            if (_folderStruc.GetList(x => x.MotherFolderId == motherId && x.text == text).Any())
            {
                message = "Folder name already Exist";
            }

            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
            {
                message = "Folder name required";
            }


            return message;

        }

        [HttpPost]
        public async Task<IActionResult> UpdateDirectoryFolderName(Guid MotherId, string text)
        {

            try
            {
                var validationMessage = validateUpdateFolder(MotherId, text);
                if (String.IsNullOrEmpty(validationMessage) || String.IsNullOrWhiteSpace(validationMessage))
                {
                    var updatedAdded = await _folderStruc.GetDetailsAsync(MotherId);
                    updatedAdded.text = text;
                    await _folderStruc.UpdateEntityAsync(updatedAdded);
                    return Ok(updatedAdded);
                }
                else
                {
                    return BadRequest(validationMessage);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewDirectoryFolder(Guid MotherId, string text)
        {
            FolderStructureModel mod = new FolderStructureModel() { text = text, MotherFolderId = MotherId };
            try
            {

                var validationMessage = validateNewFolder(MotherId, text);
                if (String.IsNullOrEmpty(validationMessage) || String.IsNullOrWhiteSpace(validationMessage))
                {
                    var updatedAdded = await _folderStruc.AddEntityAsync(mod);
                    return Ok(updatedAdded);
                }
                else
                {
                    return BadRequest(validationMessage);
                }


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }

        public IActionResult GetCoreFolder()
        {
            return Json(_folderStruc.GetCoreFolders());
        }

        public async Task<IActionResult> SaveAndSend(Guid Id)
        {
            var doc = _documentRepo.GetDetails(x => x.Id == Id);
            if (doc == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.signatories = doc.AssignedSignatories?.Select(c => new SelectListItem() { Text = String.Format("{0}<{1}>", c.Name, c.EmailAddress), Value = c.Id.ToString() }).ToList();

            ViewBag.DocumentId = String.Format("{0}.{1}", doc.Id, "pdf");
            ViewBag.IsAutoSend = "true";
            return View("PdfDetails", doc);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Document doc)
        {     
            
            
            
            var d = await _documentRepo.AddEntityAsync(doc);


            return RedirectToAction("SaveAndSend", new { Id = d.Id });

            // return RedirectToAction("Details", new { Id = d.Id });
        }
        public IActionResult Details(Guid Id)
        {
            var doc = _documentRepo.GetDetails(x => x.Id == Id);
            if (doc == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.signatories = doc.AssignedSignatories?.Select(c => new SelectListItem() { Text = String.Format("{0}<{1}>", c.Name, c.EmailAddress), Value = c.Id.ToString() }).ToList();
            ViewBag.IsAutoSend = "false";
            ViewBag.DocumentId = String.Format("{0}.{1}", doc.Id, "pdf");
            return View("PdfDetails", doc);
        }


        public async Task<IActionResult> DownloadFileRepository(Guid Id)
        {

            try
            {
                var file = await _fileRepository.GetFileRepositoryPath(Id);

                byte[] fileBytes = System.IO.File.ReadAllBytes(file.FullPath);
                string fileName = file.Filename;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);






            }
            catch (Exception e)
            {
                return BadRequest("error:" + e.ToString());
            }



        }




        public async Task<IActionResult> ViewFileRepository(Guid Id)
        {

            try
            {
                var file = await _fileRepository.GetFileRepositoryPath(Id);

                return File(file.VirtualPath, file.ContentType);


            }
            catch (Exception e)
            {
                return BadRequest("error:" + e.ToString());
            }



        }


        public IActionResult AddNewSupportingAttachments()
        {
            return View();
        }


        public IActionResult AddNewSignatories()
        {
            return View();
        }


        [HttpPost]
        public async Task<DocumentElementCoordinates> AddSignatureElement(Guid SignatoryId, Guid DocumentId, int PageNumber, float pageX, float pageY)
        {

            DocumentElementCoordinates elem = new DocumentElementCoordinates() { ElementType = "Signature", DocumentId = DocumentId, SignatoryId = SignatoryId, PageNumber = PageNumber, PageX = pageX, PageY = pageY };
            var result = await _elementCoor.AddEntityAsync(elem);
            return result;
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCoordinates(DocumentElementCoordinates coor)
        {

            var elemCoor = _elementCoor.GetDetails(x => x.Id == coor.Id);
            elemCoor.PageX = coor.PageX;
            elemCoor.PageY = coor.PageY;
            await _elementCoor.UpdateEntityAsync(elemCoor);
            return Json(new { success = true });
        }


        public IActionResult GetExistingelementCoordinates(Guid SignatoryId, Guid DocumentId, int PageNumber)
        {
            var coordinates = _elementCoor.GetList(x => x.SignatoryId == SignatoryId && x.DocumentId == DocumentId && x.PageNumber == PageNumber);
            return View("ListOfElements", coordinates); ;
        }


        public IActionResult GetExistingelementCoordinatesForUpdate(Guid SignatoryId, Guid DocumentId, int PageNumber)
        {
            var coordinates = _elementCoor.GetList(x => x.DocumentId == DocumentId && x.PageNumber == PageNumber);


            //foreach(var item in coordinates)
            // {
            //     item.PageY -= 15;
            // }
            ViewBag.CurrentUser = getUserEmail();
            return View("ListOfElementsForUpdate", coordinates);
        }







        [HttpDelete]
        public async Task<IActionResult> DeleteCoordinates(Guid Id)
        {

            await _elementCoor.PermanentDeleteEntity(Id);

            return Json(new { success = true });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGatheredSignature(Guid currentTransaction, string dataUrlString)
        {

            var coordinate = await _elementCoor.GetDetailsAsync(currentTransaction);
            if (coordinate != null)
            {
                coordinate.base64signature = dataUrlString;
                await _elementCoor.UpdateEntityAsync(coordinate);
            }
            else
            {
                throw new Exception("Transaction Cannot be found");
            }
            return View("SignatureElementForUpdateWithSignaOwn", coordinate);
        }
        [HttpPost]
        public async Task<IActionResult> ClearTransactionSignature(Guid Id)
        {
            var result = await _elementCoor.PermanentDeleteEntity(Id);
            return Json(new { success = result });
        }




        public IActionResult ExtractReport(Guid Id)
        {
            var document = _context.Documents.Find(Id);
            if (document != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "Files", "PDF");
                var filePath = Path.Combine(uploads, String.Format("{0}.{1}", document.Id.ToString(), "pdf"));
                var filename = String.Format("{0}-{1}", document.Id.ToString(), document.Filename);
                var resultPath = Path.Combine(uploads, "Extracted", String.Format("{0}", filename));

                using (Stream inputPdfStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (Stream outputPdfStream = new FileStream(resultPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var reader = new PdfReader(inputPdfStream);
                    var stamper = new PdfStamper(reader, outputPdfStream);
                    PdfContentByte pdfContentByte = null;

                    int c = reader.NumberOfPages;


                    for (int i = 1; i <= c; i++)
                    {

                        var listForRender = document.SignatoriesCoordinate.Where(x => x.PageNumber == i).ToList();
                        if (listForRender != null && listForRender.Count() > 0)
                        {
                            foreach (var item in listForRender)
                            {
                                if (item.base64signature != null)
                                {

                                    iTextSharp.text.Image image = null;
                                    var finalBase64 = item.base64signature.Substring(item.base64signature.LastIndexOf(',') + 1);
                                    var byteImage = Convert.FromBase64String(finalBase64);

                                    image = iTextSharp.text.Image.GetInstance(byteImage);
                                    pdfContentByte = stamper.GetOverContent(i);
                                    image.ScaleToFit(200, 45);
                                    image.SetAbsolutePosition(((float)item.PageX), ((float)item.PageY) - 65);
                                    pdfContentByte.AddImage(image);
                                }
                            }
                        }

                    }
                    stamper.Close();
                }
                return File(String.Format("{0}{1}", "~/Files/PDF/Extracted/", filename), "application/pdf");
            }
            return null;
        }

    }
}
