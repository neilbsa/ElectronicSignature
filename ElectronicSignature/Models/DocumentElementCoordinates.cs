using ElectronicSignature.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.Models
{
    public class DocumentElementCoordinates : IBaseEntity
    {
        public Guid Id { get;set; }
        public bool IsDeleted { get;set; }

        public string ElementType { get; set; }
        public int PageNumber { get; set; }
        public double PageX { get; set; }
        public double PageY { get; set; }

        public Guid DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document DocumentDetails { get; set; }

        public Guid SignatoryId { get; set; }
        [ForeignKey("SignatoryId")]
        public virtual Signatories SignatoryDetails { get; set; }
        public string base64signature { get; set; }


    }
}
