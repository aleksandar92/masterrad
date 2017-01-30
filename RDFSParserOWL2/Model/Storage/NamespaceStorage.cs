using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model.Storage
{
    class NamespaceStorage
    {

        private List<Namespace> namespaces;

        public NamespaceStorage(List<Namespace> ns) 
        {
            namespaces = ns;
        }

        public List<Namespace> Namespaces
        {
            get { return namespaces; }
            set { namespaces = value; }
        }

        public void ProcessNamespaces(string baseUri,List<Namespace> nspaces) 
        {
            List<Namespace> namespacesToGenerate = namespaces.Where(x => x.IsToBeGenerated == true).ToList();
            foreach (Namespace n in namespacesToGenerate)
            {
                 n.ProcessNamespace(baseUri);
            }
            RewriteNamespaces(nspaces);
        }

        private void  RewriteNamespaces(List<Namespace> nspaces) 
        {
            if (nspaces == null)
                return;

            foreach(Namespace ns in nspaces) 
            {
                Namespace namesp = namespaces.Where(x=>x.Prefix!=null && x.Prefix.Equals(ns.Prefix)).SingleOrDefault();
                if (namesp != null)
                {
                    namesp.Value = ns.Value;
                }
                else 
                {
                    namespaces.Add(namesp);
                }
            }
        }

        public string GetNamespaceValue( string ns) 
        {
            return namespaces.Where(x => x.Prefix != null && x.Prefix.Equals(ns)).Single().Value;
        }

        

    }
}
