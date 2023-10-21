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
        readonly ComponentContainer container;

        public MainClass()
        {
            entities = new Entity[defaultBufferSize];
            container = ComponentContainer.GetInstance();
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
    }
}
