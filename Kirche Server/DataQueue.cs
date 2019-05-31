using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransitPackage;

namespace Kirche_Server
{
    class DataQueue
    {
        public List<List<KircheElem>> UpdatedElems { get; set; }
        public List<Dictionary<int,string>> DeletedElems { get; set; }

        public DataQueue()
        {
            UpdatedElems = new List<List<KircheElem>>();
            DeletedElems = new List<Dictionary<int,string>>();
        }
    }
}
