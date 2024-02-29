using System.Collections.Generic;

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
