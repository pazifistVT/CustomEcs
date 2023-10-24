using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEcs
{
    public class MainClass
    {
        static int defaultBufferSize = 8;

        internal Entity[] entities;
        internal List<ISystem> systems;
        readonly ComponentContainer container;

        public MainClass()
        {
            entities = new Entity[defaultBufferSize];
            container = ComponentContainer.GetInstance();
            systems = new List<ISystem>();
        }

        public Entity CreateEntity()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if(entities[i] == null)
                {
                    entities[i] = new Entity(i);
                    return entities[i];
                }
                else if(!entities[i].IsAlive)
                {
                    return entities[i].ActivateEntity(i);
                }
            }
            int index = entities.Length;
            Array.Resize(ref entities, entities.Length * 2);
            entities[index] = new Entity(index);
            return entities[index];
        }

        public void RegistrationSystem(ISystem system)
        {
            systems.Add(system);
            List<BaseFilter> filtersThisSystem = system.Initialization(this);
            system.Filters = filtersThisSystem.ToArray();
            foreach (BaseFilter filter in system.Filters)
            {
                filter.mainClass = this;
            }
        }

        public void Update()
        {
            foreach (ISystem item in systems)
            {
                foreach (BaseFilter filter in item.Filters)
                {
                    filter.UpdateFilter();
                }
                item.Update();
            }
        }
    }
}
