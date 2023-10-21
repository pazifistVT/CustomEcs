using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEcs
{
    public abstract class BaseFilter
    {
        public abstract void UpdateFilter();
    }
    public class Filter<T> : BaseFilter  where T : struct
    {
        static int HashType = Component<T>.GetInstanceComponent().HashType;

        MainClass mainClass;
        public List<int> FilteredEntities { get; private set; }

        public override void UpdateFilter()
        {
            FilteredEntities = new List<int>();

            for (int i = 0; i < mainClass.entities.Length; i++)
            {
                if (mainClass.entities[i] != null && mainClass.entities[i].IsAlive) 
                {
                    if(mainClass.entities[i].CheckComponentType(HashType))
                    {
                        FilteredEntities.Add(mainClass.entities[i].IndexEntity);
                    }
                }
            }
        }

        public Entity GetEntity(int index)
        {
            return mainClass.entities[index];
        }

        public ref T GetComponent(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<T>();
        }
    }
}
