using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Common
{
    static class ConfigurationFiles
    {
        private static HashSet<string> stereotypesToSkip = new HashSet<string>(InputOutput.LoadFixedStereotypes());


        public static HashSet<string> StereotypesToSkip 
        {
            get { return stereotypesToSkip; }
        }
    }
}
