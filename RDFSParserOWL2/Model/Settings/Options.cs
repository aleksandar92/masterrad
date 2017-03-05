using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model.Settings
{
    public enum REGIMESOFWORK { COMMONEXTENSION, EXTENSION, COMMON, BASE } 
    public enum EnumRepresentationKindOWL2:int {OPENED,CLOSED}
    public enum CIMDatatypePresentation : int { SIMPLE, COMPLEX }

     public  class Options
    {
        private EnumRepresentationKindOWL2 enumLook;
        private CIMDatatypePresentation cimDatatypePresentation;

        public CIMDatatypePresentation CimDatatypePresentation
        {
            get { return cimDatatypePresentation; }
            set { cimDatatypePresentation = value; }
        }

        public Options(EnumRepresentationKindOWL2 eo,CIMDatatypePresentation cdp) 
        {
            enumLook = eo;
            cimDatatypePresentation = cdp;
        }

        public EnumRepresentationKindOWL2 EnumLook
        {
            get { return enumLook; }
            set { enumLook = value; }
        }


    }
}
