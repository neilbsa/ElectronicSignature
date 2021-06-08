using ElectronicSignature.Core.Repository.Core.Implementations;
using ElectronicSignature.Data;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Implementation
{
    public class DocumentFolderLink : Repository<FolderStrucDocumentLink>, IDocumentFolderLink
    {
        public ApplicationDbContext _context { get; set; }
        public IDocumentRepository _docs { get; set; }
        public IFolderStructureService _folderStruc { get; set; }

        public DocumentFolderLink(ApplicationDbContext cont):base(cont)
        {
        
        }


        public async Task AddFileToFolder(FolderStructureModel FolderId, Document DocumentId)
        {

       
            if (DocumentId == null)
            {
                throw new Exception("Document cannot found");
            }

            if (FolderId == null)
            {
                throw new Exception("Folder is Invalid");
            }


            if(DocumentId != null && FolderId != null)
            {
               await AddEntityAsync(new FolderStrucDocumentLink() { DocumentId = DocumentId.Id, FolderStrucId = FolderId.Id });
            }

        }
    }
}
