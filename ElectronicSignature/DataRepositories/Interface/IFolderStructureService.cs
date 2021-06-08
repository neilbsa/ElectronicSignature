using ElectronicSignature.Core.Repository.Core.Interfaces;
using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Interface
{
    public interface IFolderStructureService : IRepository<FolderStructureModel>
    {
        Task CreateInitialFolder(string userEmail);
        Task CreateFolderUnderFolder(Guid MotherId, string FolderName);
        Task DeleteFolder(Guid Id);
        Guid[] GetCoreFolders();
        Task<FolderStructureModel> GetDeleteFolderId(string userEmail);
        Task<FolderStructureModel> GetDefaultFolderId(string userEmail);
    }
}
