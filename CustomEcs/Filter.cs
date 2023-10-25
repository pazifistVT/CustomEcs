using System.Collections.Generic;

namespace CustomEcs
{
    public abstract class BaseFilter
    {
        internal abstract void UpdateFilter();
        internal MainClass mainClass;
    }
    public class Filter<T> : BaseFilter where T : struct
    {
        static Filter<T> filter;
        private static readonly int HashType = Component<T>.GetInstanceComponent().HashType;
        
        public List<int> FilteredEntities { get; private set; }

        public static Filter<T> GetFilter()
        {
            if (filter == null)
            {
                filter = new Filter<T>();
            }
            return filter;
        }

        internal override void UpdateFilter()
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
