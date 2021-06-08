using ElectronicSignature.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicSignature.Models
{
    public class DocumentSignatoriesCoordinateLink : IBaseEntity
    {
        public Guid Id { get;set; }
        public bool IsDeleted { get;set; }
        
   



    }
}
