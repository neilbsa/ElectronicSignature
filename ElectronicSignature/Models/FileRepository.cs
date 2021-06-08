using ElectronicSignature.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicSignature.Models
{
    public class FileRepository : IChangeTracker
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }

        
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public double ContentSize { get; set; }

        public Guid DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document DocumentDetails { get; set; }
        public DateTime CreateDate {  get;set; }
        public DateTime LastModifiedDate {  get;set; }
        public string LastModifiedUser {  get;set; }
        public string CreateUser {  get;set; }

        [NotMapped]
        public string VirtualPath { get; set; }

        [NotMapped]
        public string FullPath { get; set; }



    }
}
