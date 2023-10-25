using System;

namespace CustomEcs
{
    public class Entity
    {
        static int defaultSizeBuffer = 8;

        readonly ComponentContainer container;
        int[] IndexesComponents;
        public bool IsAlive { get; private set; }
        public int IndexEntity { get; private set; }
        public Entity(int indexEntity)
        {
            IndexesComponents = new int[defaultSizeBuffer * 2];
            for (int i = 0; i < IndexesComponents.Length; i++)
            {
                IndexesComponents[i] = -1;
            }
            IsAlive = true;
            container = ComponentContainer.GetInstance();
            this.IndexEntity = indexEntity;
        }

        internal Entity ActivateEntity(int indexEntity)
        {
            IndexesComponents = new int[defaultSizeBuffer * 2];
            for (int i = 0; i < IndexesComponents.Length; i++)
            {
                IndexesComponents[i] = -1;
            }
            IsAlive = true;
            this.IndexEntity = indexEntity;
            return this;
        }

        public void DeleteEntity()
        {
            IsAlive = false;
        }

        public ref T AddOrGetComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent();
            int HashType = componentClass.HashType;
            int indexNewComponents = -1;
            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                if (IndexesComponents[i] == HashType)
                {
                    int indexComponents = i + 1;
                    return ref componentClass.GetComponent(IndexesComponents[indexComponents], IndexEntity);
                }
                if (IndexesComponents[i] == -1)
                {
                    indexNewComponents = i;
                }
            }
            if (indexNewComponents >= 0)
            {
                int indexComponents = indexNewComponents + 1;
                IndexesComponents[indexNewComponents] = HashType;
                return ref componentClass.AddComponent(out IndexesComponents[indexComponents], IndexEntity);
            }
            else
            {
                int index = IndexesComponents.Length;
                Array.Resize(ref IndexesComponents, IndexesComponents.Length * 2);
                int indexComponents = index + 1;
                IndexesComponents[index] = HashType;
                return ref componentClass.AddComponent(out IndexesComponents[indexComponents], IndexEntity);
            }
        }

        public void DeleteComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent();
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

            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                if (IndexesComponents[i] >= 0)
                {
                    return;
                }
            }

            DeleteEntity();
        }

        internal bool CheckComponentType(int HashType)
        {
            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                if (IndexesComponents[i] == HashType)
                {
                    return true;
                }
            }
            return false;
        }
    }

}


