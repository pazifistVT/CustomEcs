using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEcs
{
    internal class SerializeComponent
    {
        public int componentType;
        public string value;
    }

    internal class SerializeListComponents
    {
        public List<SerializeComponent> list;
    }
}
