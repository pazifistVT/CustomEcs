using System;
using System.Collections.Generic;
using System.Text.Json;
namespace CustomEcs
{
    public class MainClassECS
    {
        static readonly int defaultBufferSize = 8;

        internal int firstFreeIndex;
        internal int lastFreeIndex;
        internal Entity[] entities;
        internal List<SystemECS> systems;
        private ComponentContainer container;

        internal class MainClassJson
        {
            public string container;
            public List<string> entities;
            public int firstFreeIndex;
            public int lastFreeIndex;
        }

        internal string Serialize()
        {
            List<string> entitiesJson = new List<string>();
            foreach (Entity item in entities)
            {
                entitiesJson.Add(item.Serialize());
            }
            MainClassJson entity = new MainClassJson()
            {
                firstFreeIndex = firstFreeIndex,
                lastFreeIndex = lastFreeIndex,
                container = container.Serialize(),
                entities = entitiesJson
            };
            string s = JsonSerializer.Serialize<MainClassJson>(entity);
            return s;
        }

        internal void Deserialize(string s)
        {
            MainClassJson entity = new MainClassJson();
            try
            {
                entity = JsonSerializer.Deserialize<MainClassJson>(s);
            }
            catch (Exception e)
            {

            }
            List<Entity> entitiesFromJson = new List<Entity>();
            foreach (string item in entity.entities)
            {
                Entity e = new Entity(0, this);
                e.Deserialize(item);
                entitiesFromJson.Add(e);
            }


            firstFreeIndex = entity.firstFreeIndex;
            lastFreeIndex = entity.lastFreeIndex;

            container = ComponentContainer.GetInstance(this);
            container.Deserialize(entity.container);
            entities = entitiesFromJson.ToArray();
        }

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
