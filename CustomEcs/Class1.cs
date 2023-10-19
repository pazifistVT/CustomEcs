using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEcs
{
    public class MainClass
    {
        Entity[] entities;
        readonly ComponentContainer container;

        public MainClass()
        {
            entities = new Entity[8];
            container = ComponentContainer.GetInstance();
        }

        public Entity CreateEntity()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if(entities[i] == null)
                {
                    entities[i] = new Entity();
                    return entities[i];
                }
                else if(!entities[i].isAlive)
                {
                    return entities[i].ActivateEntity();
                }
            }
            int index = entities.Length;
            Array.Resize(ref entities, entities.Length * 2);
            entities[index] = new Entity();
            return entities[index];
        }
    }

    class ComponentContainer
    {
        private static ComponentContainer container;

        List<BaseComponent> componentsClass = new List<BaseComponent>();

        private ComponentContainer()
        {
            
        }

        public static ComponentContainer GetInstance()
        {
            if(container == null)
            {
                container = new ComponentContainer();
            }
            return container;
        }

        public bool CheckClass(int HashType)
        {
            foreach (BaseComponent item in componentsClass)
            {
                if (item.HashType == HashType)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddComponentClass<T>(Component<T> component) where T : struct
        {
            componentsClass.Add(component);
        }
    }

    abstract class BaseComponent
    {
        public int HashType { get; set; }

    }

    class Component<T> : BaseComponent where T : struct
    {
        static int defaultSizeBuffer = 32;
        static Component<T> obj;

        public static Component<T> GetInstanceComponent(ComponentContainer componentContainer)
        {
            if(obj == null)
            {
                obj = new Component<T>();
                obj.components = new T[defaultSizeBuffer];
                obj.aliveComponents = new bool[defaultSizeBuffer];

                obj.HashType = obj.GetType().GetHashCode();

                if(!componentContainer.CheckClass(obj.HashType))
                {
                    componentContainer.AddComponentClass(obj);
                }

                return obj;
            }
            else
            {
                return obj;
            }
        }

        T[] components;
        bool[] aliveComponents;

        public ref T CreateNewComponent(int index)
        {
            components[index] = new T();
            aliveComponents[index] = true;
            return ref components[index];
        }

        public ref T GetComponent(in int index)
        {
            if(aliveComponents[index])
            {
                return ref components[index];
            }
            else
            {
                return ref CreateNewComponent(index);
            }
        }

        public ref T AddComponent(out int indexNewComponent)
        {
            indexNewComponent = aliveComponents.Length;
            for (int i = 0; i < aliveComponents.Length; i++)
            {
                if(!aliveComponents[i])
                {
                    return ref CreateNewComponent(i);
                }
            } 
            Array.Resize(ref aliveComponents, aliveComponents.Length * 2);
            Array.Resize(ref components, components.Length * 2);
            return ref CreateNewComponent(indexNewComponent);
        }

        public void DeleteComponent(in int index)
        {
            aliveComponents[index] = false;
        }
    }

    public class Entity
    {
        static int defaultSizeBuffer = 8;

        readonly ComponentContainer container;
        int[] IndexesComponents;
        public bool isAlive;

        public Entity()
        {
            IndexesComponents = new int[defaultSizeBuffer * 2];
            isAlive = true;
            container = ComponentContainer.GetInstance();

        }

        public Entity ActivateEntity()
        {
            IndexesComponents = new int[defaultSizeBuffer * 2];
            isAlive = true;
            return this;
        }

        public ref T AddComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent(container);
            int HashType = componentClass.HashType;
            int indexNewComponents = -1;
            for (int i = 0; i < IndexesComponents.Length; i+=2)
            {
                if (IndexesComponents[i] == HashType)
                {
                    int indexComponents = i + 1;
                    return ref componentClass.GetComponent(IndexesComponents[indexComponents]);
                }
                if (IndexesComponents[i] == -1)
                {
                    indexNewComponents = i;
                }
            }
            if(indexNewComponents >= 0)
            {
                int indexComponents = indexNewComponents + 1;
                IndexesComponents[indexNewComponents] = HashType;
                return ref componentClass.AddComponent(out IndexesComponents[indexComponents]);
            }
            else
            {
                int index = IndexesComponents.Length;
                Array.Resize(ref IndexesComponents, IndexesComponents.Length * 2);
                int indexComponents = index + 1;
                IndexesComponents[indexNewComponents] = HashType;
                return ref componentClass.AddComponent(out IndexesComponents[indexComponents]);
            }
        }

        public void DeleteComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent(container);
            int HashType = componentClass.HashType;
            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                if (IndexesComponents[i] == HashType)
                {
                    IndexesComponents[i] = -1;
                    int indexComponents = i + 1;
                    componentClass.DeleteComponent(in IndexesComponents[indexComponents]);
                }
            }
        }
    }
}
