using ElectronicSignature.Core.Repository.Core.Implementations;
using ElectronicSignature.Data;
using ElectronicSignature.DataRepositories.Interface;
using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Implementation
{
    public class DocumentSignatories : Repository<Signatories> , ISignatories
    {

        public ApplicationDbContext _context { get; set; }
        public DocumentSignatories(ApplicationDbContext cont) : base(cont)
        {
            _context = cont;
        }






    }
}
