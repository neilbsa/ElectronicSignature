﻿using ElectronicSignature.Core.Repository.Core.Interfaces;
using ElectronicSignature.Data;
using ElectronicSignature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicSignature.DataRepositories.Interface
{
    public interface IElementCoordinates : IRepository<DocumentElementCoordinates>
    {


        List<DocumentElementCoordinates> GetAllSignatureTemplate(string Email);

    }
}
