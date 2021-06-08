using ElectronicSignature.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.Models
{
    public class Signatories : IBaseEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public int OrderNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public Guid DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document DocumentDetails { get; set; }
        public virtual List<SignatureTemplates> TemplateSignature { get; set; }


    }
}
