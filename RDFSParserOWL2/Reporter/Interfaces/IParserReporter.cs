using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter.Interfaces
{

    public enum EntityTypesReporter
    {
        Class,
        ClassCategory,
        Property,
        EnumMembers,
        Unknown

    }

    public interface IParserReporter : IReporter
    {

        int EntityCount{get;}
        int EntityCountByType(EntityTypesReporter type);
        void AddtoEntityCountByType(EntityTypesReporter type, int number);
        void RemoveFromEntityCountByType(EntityTypesReporter type, int number);
    }
}
