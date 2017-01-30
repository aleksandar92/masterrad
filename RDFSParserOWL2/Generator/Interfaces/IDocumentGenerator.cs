using RDFSParserOWL2.Model;
using RDFSParserOWL2.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Generator.Interfaces
{
    public  interface IDocumentGenerator
    {
        void GenerateDocument(Profile profileForGenerating);
    }
}
