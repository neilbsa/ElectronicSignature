using ElectronicSignature.Core.Repository.Core.Implementations;
using ElectronicSignature.Data;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Implementation
{
    public class FoldereStructureRepository : Repository<FolderStructureModel>, IFolderStructureService
    {

        public ApplicationDbContext _context { get; set; }
        public IHttpContextAccessor _httpAccessor { get; set; }
        public IDocumentRepository _document { get; set; }
        public string[] CoreFolder { get; set; } = new string[] { "Documents", "Archives", "Deleted-Items" };

        public FoldereStructureRepository(ApplicationDbContext cont, IHttpContextAccessor access) : base(cont)
        {
            _httpAccessor = access;
           
        }
        public async Task CreateInitialFolder(string userEmail)
        {
            var currentStats = GetList(x=>x.OwnerUserEmail == userEmail && x.MotherFolderId == null);

            if (currentStats == null || currentStats.Count() == 0)
            {
                foreach (var item in CoreFolder)
                {
                    FolderStructureModel mod = new FolderStructureModel() { text = item,OwnerUserEmail = userEmail };
                    await AddEntityAsync(mod);
                }
            }
        }

        private string getUserEmail()
        {

            return _httpAccessor.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Email).Value;
        }


        public async Task CreateFolderUnderFolder(Guid MotherId, string FolderName)
        {
            var motherDetails = await GetDetailsAsync(MotherId);
            if (motherDetails != null)
            {
                if (motherDetails.children == null || !motherDetails.children.Any(x => x.text == FolderName))
                {
                    FolderStructureModel mod = new FolderStructureModel() { MotherFolderId = MotherId, text = FolderName };
                    await AddEntityAsync(mod);
                }
                else
                {
                    throw new Exception("FolderName Already Exist");
                }
            }
        }

        public async Task DeleteFolder(Guid Id)
        {
            var motherDetails = await GetDetailsAsync(Id);
            if (motherDetails != null)
            {


               
            }

        }

        public async Task<FolderStructureModel> GetDefaultFolderId(string userEmail)
        {
            var documentDetails = GetDetails(x => x.text == "Documents" && x.OwnerUserEmail == userEmail);
        
            if(documentDetails == null)
            {
              await  CreateInitialFolder(userEmail);
                documentDetails = GetDetails(x => x.text == "Documents" && x.OwnerUserEmail == userEmail);
            }
          
            return documentDetails;
        }

        public async Task<FolderStructureModel> GetDeleteFolderId(string userEmail)
        {
            var documentDetails = GetDetails(x => x.text == "Deleted-Items" && x.OwnerUserEmail == userEmail);

            if (documentDetails == null)
            {
                await CreateInitialFolder(userEmail);
                documentDetails = GetDetails(x => x.text == "Deleted-Items" && x.OwnerUserEmail == userEmail);
            }

            return documentDetails;
        }

        public Guid[] GetCoreFolders()
        {
            var opt = GetList(x => x.MotherFolderId == null && x.OwnerUserEmail == getUserEmail()).Select(x=>x.Id).ToArray();
            return opt;

        }
    }
}
