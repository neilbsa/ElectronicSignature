using ElectronicSignature.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicSignature.Models
{
    public class SignatureTemplates : IBaseEntity
    {
        public Guid Id { get;set; }
        public bool IsDeleted { get;set; }        
        public Guid SignatoryId { get; set; }        
        [ForeignKey("SignatoryId")]
        public virtual Signatories SignatoryDetails { get; set; }
        public string base64stringFile { get; set; }

    }
}
