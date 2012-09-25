using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlueCsvEditor.KnownValues
{
    public interface IKnownValueRetriever
    {
        IEnumerable<string> GetKnownValues(string fullTypeName);
    }
}
