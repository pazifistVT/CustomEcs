using System;
using System.Collections.Generic;

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
            container = ComponentContainer.GetInstance(this);
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
            List<BaseFilter> filtersThisSystem = system.Initialization();
            system.Filters = filtersThisSystem.ToArray();
            system.MainClass = this;
            foreach (BaseFilter filter in system.Filters)
            {
                filter.mainClass = this;
            }
        }

        public void Initialization()
        {

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

        internal Entity GetEntity(in int index)
        {
            return entities[index];
        }
    }
}
