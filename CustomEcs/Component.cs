using System;

namespace CustomEcs
{
    class Component<T> : BaseComponent where T : struct
    {
        static int defaultSizeBuffer = 32;
        static Component<T> obj;

        public static Component<T> GetInstanceComponent(ComponentContainer componentContainer)
        {
            if (obj == null)
            {
                obj = new Component<T>();
                obj.components = new T[defaultSizeBuffer];
                obj.aliveComponents = new bool[defaultSizeBuffer];
                obj.indexesEmpty = new int[defaultSizeBuffer];

                obj.HashType = obj.GetType().GetHashCode();

                if (!componentContainer.CheckClass(obj.HashType))
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

        public static Component<T> GetInstanceComponent()
        {
            return obj;
        }

        T[] components;
        bool[] aliveComponents;
        int[] indexesEmpty;
        public ref T CreateNewComponent(int index, int entityIndex)
        {
            components[index] = new T();
            aliveComponents[index] = true;
            indexesEmpty[index] = entityIndex;
            return ref components[index];
        }

        public ref T GetComponent(in int index, int entityIndex)
        {
            if (aliveComponents[index])
            {
                indexesEmpty[index] = entityIndex;
                return ref components[index];
            }
            else
            {
                return ref CreateNewComponent(index, entityIndex);
            }
        }

        public ref T AddComponent(out int indexNewComponent, int entityIndex)
        {
            indexNewComponent = aliveComponents.Length;
            for (int i = 0; i < aliveComponents.Length; i++)
            {
                if (!aliveComponents[i])
                {
                    return ref CreateNewComponent(i, entityIndex);
                }
            }
            Array.Resize(ref aliveComponents, aliveComponents.Length * 2);
            Array.Resize(ref components, components.Length * 2);
            return ref CreateNewComponent(indexNewComponent, entityIndex);
        }

        public void DeleteComponent(in int index)
        {
            aliveComponents[index] = false;
        }
    }
}

