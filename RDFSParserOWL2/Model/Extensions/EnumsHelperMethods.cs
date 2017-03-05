
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model.Extensions
{
    public class EnumsHelperMethods
    {


        public static EnumRepresentationKindOWL2 ToEnumOptions(Boolean options)
        {
            switch (options)
            {
                case false:
                    return EnumRepresentationKindOWL2.CLOSED;
                case true:
                    return EnumRepresentationKindOWL2.OPENED;
                default:
                    return EnumRepresentationKindOWL2.CLOSED;
            }
        }

        public static CIMDatatypePresentation ToCimDatatypePresentation(Boolean options)
        {
            switch (options)
            {
                case false:
                    return CIMDatatypePresentation.COMPLEX;
                case true:
                    return CIMDatatypePresentation.SIMPLE;
                default:
                    return CIMDatatypePresentation.SIMPLE;
            }
        }




        public static REGIMESOFWORK ToRegimesOfWork(Boolean extension, Boolean common)
        {
            if (extension && common) 
            {
                return REGIMESOFWORK.COMMONEXTENSION;
            }
            else if (extension) 
            {
                return REGIMESOFWORK.EXTENSION;
            }
            else if (common)
            {
                return REGIMESOFWORK.COMMON;
            }
            else 
            {
                return REGIMESOFWORK.BASE;
            }
        }

    }
}
