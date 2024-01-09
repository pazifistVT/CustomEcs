using System;
using System.Diagnostics.Eventing.Reader;

namespace CustomEcs
{
    public class Entity
    {
        static readonly int defaultSizeBuffer = 8;

        readonly ComponentContainer container;
        readonly MainClassECS mainClass;
        int[] IndexesComponents;
        public bool IsAlive { get; private set; }
        public int IndexEntity { get; private set; }
        internal Entity(int indexEntity, MainClassECS mainClass)
        {
            IndexesComponents = new int[defaultSizeBuffer * 2];
            for (int i = 0; i < IndexesComponents.Length; i++)
            {
                IndexesComponents[i] = -1;
            }
            IsAlive = true;
            container = ComponentContainer.GetInstance();
            this.IndexEntity = indexEntity;
            this.mainClass = mainClass;
        }

        internal Entity ActivateEntity(int indexEntity)
        {
            //IndexesComponents = new int[defaultSizeBuffer * 2];
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
            //Перебираем индексы типов структур и удаляем компоненты
            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                if(IndexesComponents[i] >= 0)
                {
                    int indexComponents = i + 1;
                    container.GetComponent(IndexesComponents[i]).DeleteComponent(indexComponents);
                }
                
            }
            mainClass.lastDeletedEntity = IndexEntity;
        }

        public ref T AddOrGetComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent();
            int HashType = componentClass.HashType;
            int indexNewComponents = -1;

            //Перебираем индексы типов структур
            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                //Если структура уже прикреплена то возвращем ссылку на нее
                if (IndexesComponents[i] == HashType)
                {
                    int indexComponents = i + 1;
                    return ref componentClass.GetComponent(IndexesComponents[indexComponents], IndexEntity);
                }
                //Находим первый попавшийся свободный индекс
                if (IndexesComponents[i] == -1 && indexNewComponents == -1)
                {
                    indexNewComponents = i;
                }
            }
            //Если компонент не найден то создаем компонент на месте найденного индекса иначе расширяем контейнер
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

        //Удаление компонента по типу структуры
        public void DeleteComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent();
            int HashType = componentClass.HashType;
            bool componentsExist = false;
            for (int i = 0; i < IndexesComponents.Length; i += 2)
            {
                if (IndexesComponents[i] == HashType)
                {
                    int indexComponents = i + 1;
                    componentClass.DeleteComponent(in IndexesComponents[indexComponents]);
                    IndexesComponents[indexComponents] = -1;
                    IndexesComponents[i] = -1;
                }
                else if (IndexesComponents[i] >= 0)
                {
                    componentsExist = true;
                }
            }
            if (componentsExist) { return; }
            DeleteEntity();
        }

        //Проверка прикреплены ли к сущности компоненты заданного типа
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


