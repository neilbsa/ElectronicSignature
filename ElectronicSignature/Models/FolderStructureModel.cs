using ElectronicSignature.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.Models
{
    public class FolderStructureModel :IChangeTracker
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public string text { get; set; }
        public Guid? MotherFolderId { get; set; }
        [ForeignKey("MotherFolderId")]
        public virtual FolderStructureModel MotherFolderDetails { get; set; }
        public virtual List<FolderStructureModel> children { get; set; }     
        public virtual List<FolderStrucDocumentLink> Documents { get; set; }
        public string OwnerUserEmail { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedUser { get; set; }
        public string CreateUser { get; set; }
    }



    public class FolderStrucDocumentLink : IBaseEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public Guid DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document DocumentDetails { get; set; }
        public Guid FolderStrucId { get; set; }
        [ForeignKey("FolderStrucId")]
        public virtual FolderStructureModel FolderStrucDetail { get; set; }

    }


}
