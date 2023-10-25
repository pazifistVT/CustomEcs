using System.Collections.Generic;

namespace CustomEcs
{
    public interface ISystem
    {
        public MainClass MainClass { get; set; }
        public BaseFilter[] Filters { get; set; }
        public List<BaseFilter> Initialization();
        public void Update();
    }
}
