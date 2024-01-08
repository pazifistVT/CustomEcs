using System;
using System.Collections.Generic;

namespace CustomEcs
{
    public class MainClassECS
    {
        static readonly int defaultBufferSize = 8;

        internal int lastCreatedEntity;
        internal int lastDeletedEntity;
        internal Entity[] entities;
        internal List<SystemECS> systems;
        readonly ComponentContainer container;

        public MainClassECS()
        {
            entities = new Entity[defaultBufferSize];
            container = ComponentContainer.GetInstance(this);
            systems = new List<SystemECS>();
            lastCreatedEntity = 0;
        }

        public Entity CreateEntity()
        {
            if (entities[lastDeletedEntity] != null && !entities[lastDeletedEntity].IsAlive)
            {
                return entities[lastDeletedEntity].ActivateEntity(lastDeletedEntity);
            }
            for (int i = (lastCreatedEntity + 1); i < entities.Length; i++)
            {
                if(entities[i] == null)
                {
                    entities[i] = new Entity(i, this);
                    lastCreatedEntity = i;
                    return entities[i];
                }
                else if(!entities[i].IsAlive)
                {
                    lastCreatedEntity = i;
                    return entities[i].ActivateEntity(i);
                }
            }
            int index = entities.Length;
            Array.Resize(ref entities, entities.Length * 2);
            entities[index] = new Entity(index, this);
            lastCreatedEntity = index;
            return entities[index];
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
