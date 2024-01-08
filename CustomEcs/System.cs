using System.Collections.Generic;

namespace CustomEcs
{
    public abstract class SystemECS
    {
        internal BaseFilter[] Filters { get; set; }
        public MainClassECS MainClass { get; internal set; }

        public abstract List<BaseFilter> Initialization();

        public abstract void Update();
    }

}
