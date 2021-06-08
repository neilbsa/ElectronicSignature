using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.Models.Interfaces
{
   public interface IChangeTracker : IBaseEntity
    {
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedUser { get; set; }
        public string CreateUser { get; set; }

    }
}
