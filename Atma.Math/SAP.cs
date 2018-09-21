#if ATMA_PHYSICS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;

namespace Atma
{
    public interface ISAP
    {
        int min { get; }
        int max { get; }
        bool testCollision(ISAP other);
    }

    public class SAP
    {
        public struct SAPItem : IRadixKey
        {
            public ISAP item;
            public int Key { get { return item.min; } }
        }

        // array of integers to hold values
        private ISAP[] a = new ISAP[1024];

        
        
    }
}
#endif