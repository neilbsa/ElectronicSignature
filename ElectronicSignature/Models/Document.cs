using ElectronicSignature.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.Models
{
    public class Document : IChangeTracker
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedUser { get; set; }
        public string CreateUser { get; set; }
        public string Status { get; set; }
        public string Filename { get; set; }
        public string Remarks { get; set; }
        public bool IsScheduledDelivery { get; set; }

        public virtual List<FolderStrucDocumentLink> FolderIncluded { get; set; }
        public virtual List<Signatories> AssignedSignatories { get; set; }
        public virtual List<DocumentElementCoordinates> SignatoriesCoordinate { get; set; }
        public virtual List<FileRepository> SupportingDocuments { get; set; }
        [NotMapped]
        public List<IFormFile> SupportingDocsForUpload { get; set; }
        [NotMapped]
        public IFormFile uploadedFile { get; set; }

        [NotMapped]
        public bool SendNotification { get; set; }

        [NotMapped]
        public bool IsOwned { get; set; }
    }
}
