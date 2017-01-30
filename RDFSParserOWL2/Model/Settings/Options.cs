using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model.Settings
{
    public enum REGIMESOFWORK { COMMONEXTENSION, EXTENSION, COMMON, BASE } 
    public enum EnumRepresentationKindOWL2:int {OPENED,CLOSED} 

     public  class Options
    {
        private EnumRepresentationKindOWL2 enumLook;

        public Options(EnumRepresentationKindOWL2 eo) 
        {
            enumLook = eo;
        }

        public EnumRepresentationKindOWL2 EnumLook
        {
            get { return enumLook; }
            set { enumLook = value; }
        }


    }
}
