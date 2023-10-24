using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEcs
{
    public interface ISystem
    {
        public MainClass MainClass { get; set; }
        internal BaseFilter[] Filters { get; set; }
        public List<BaseFilter> Initialization(MainClass mainClass);
        public void Update();
    }
}
