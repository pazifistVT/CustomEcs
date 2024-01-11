using System;
using System.Collections.Generic;

namespace CustomEcs
{
    public class MainClassECS
    {
        static readonly int defaultBufferSize = 8;

        internal int firstFreeIndex;
        internal int lastFreeIndex;
        internal Entity[] entities;
        internal List<SystemECS> systems;
        readonly ComponentContainer container;

        public MainClassECS()
        {
            entities = new Entity[defaultBufferSize];
            container = ComponentContainer.GetInstance(this);
            systems = new List<SystemECS>();
            firstFreeIndex = 0;
            lastFreeIndex = defaultBufferSize;
        }

        public Entity CreateEntity()
        {
            for (int i = firstFreeIndex; i < lastFreeIndex; i++)
            {
                if(entities[i] == null)
                {
                    entities[i] = new Entity(i, this);
                    CheckIndex(in i);

                    return entities[i];
                }
                else if(!entities[i].IsAlive)
                {
                    CheckIndex(in i);
                    return entities[i].ActivateEntity(i);
                }
            }
            int index = entities.Length;
            Array.Resize(ref entities, entities.Length * 2);
            entities[index] = new Entity(index, this);
            CheckIndex(in index);
            lastFreeIndex *= 2;
            return entities[index];
        }

        void CheckIndex(in int i)
        {
            if (i == firstFreeIndex)
            {
                firstFreeIndex = (i + 1);
            }
        }

        public void RegistrationSystem(SystemECS system)
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
            foreach (SystemECS item in systems)
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
