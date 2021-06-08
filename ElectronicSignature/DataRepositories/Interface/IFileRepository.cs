using ElectronicSignature.Core.Repository.Core.Interfaces;
using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Interface
{
    public interface IFileRepository : IRepository<FileRepository>
    {

        Task UploadFromDocument(Document doc);

        Task<FileRepository> GetFileRepositoryPath(Guid Id);

    }
}
