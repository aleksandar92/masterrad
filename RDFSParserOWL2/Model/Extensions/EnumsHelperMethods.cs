
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

        //public static EntityTypesGeneratorReporter ToEntityTypesGenerator(ProfileElementTypes type)
        //{
        //    switch (type)
        //    {
        //        case ProfileElementTypes.Class:
        //            return EntityTypesGeneratorReporter.Class;
        //        case true:
        //            return EnumRepresentationKind.OPENED;
        //        default:
        //            return EnumRepresentationKind.CLOSED;
        //    }
        //}

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
