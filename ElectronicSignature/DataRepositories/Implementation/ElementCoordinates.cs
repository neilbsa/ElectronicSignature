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
    public class DocElementCoordinates : Repository<DocumentElementCoordinates> , IElementCoordinates
    {



        public ApplicationDbContext _context { get; set; }
        public DocElementCoordinates(ApplicationDbContext cont) : base(cont)
        {
            _context = cont;
        }

        public List<DocumentElementCoordinates> GetAllSignatureTemplate(string Email)
        {
            return GetList(x => x.SignatoryDetails.EmailAddress == Email && x.base64signature != null).OrderByDescending(x=>x.DocumentDetails.CreateDate).ToList();
        }
    }
}
