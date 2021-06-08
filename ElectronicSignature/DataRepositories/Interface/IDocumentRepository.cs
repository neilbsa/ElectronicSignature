using ElectronicSignature.Core.Repository.Core.Interfaces;
using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Interface
{
    public interface IDocumentRepository : IRepository<Document>
    {
        Task SendNotificationEmail(Guid docId);

        Task UpdateSignatories(Guid Id);

        Task TransferDocumentToFolder(Guid folderId, Guid fileId);
        List<Document> GetAllFilesInFolder(Guid Id);
        Task AddNewSignatoryToDocument(Signatories mod);
    }
}
