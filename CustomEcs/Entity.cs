using System;
using System.Text.Json;
namespace CustomEcs
{
    public class Entity
    {
        static readonly int defaultSizeBuffer = 8;

        readonly ComponentContainer container;
        readonly MainClassECS mainClass;
        private int[] typesComponents;
        private int[] indexesComponents;
        private int firstIndex;//индекс первого существующего компонента
        private int lastIndex;//индекс последнего существующего компонента
        public bool IsAlive { get; private set; }
        public int IndexEntity { get; private set; }
        private class EntityJson
        {
            public int[] typesComponents;
            public int[] indexesComponents;
            public int firstIndex;
            public int lastIndex;
            public bool IsAlive;
            public int IndexEntity;

            public EntityJson()
            {

            }
        }

        internal string Serialize()
        {
            EntityJson entity = new EntityJson()
            {
                typesComponents = typesComponents,
                indexesComponents = indexesComponents,
                firstIndex = firstIndex,
                lastIndex = lastIndex,
                IsAlive = IsAlive,
                IndexEntity = IndexEntity
            };
            string s = JsonSerializer.Serialize<EntityJson>(entity);
            return s;
        }

        internal void Deserialize(string s)
        {
            EntityJson entity = new EntityJson();
            try
            {
                entity = JsonSerializer.Deserialize<EntityJson>(s);
            }
            catch (Exception e)
            {

            }
            typesComponents = entity.typesComponents;
            indexesComponents = entity.indexesComponents;
            firstIndex = entity.firstIndex;
            lastIndex = entity.lastIndex;
            IsAlive = entity.IsAlive;
            IndexEntity = entity.IndexEntity;
        }

        internal Entity(int indexEntity, MainClassECS mainClass)
        {
            typesComponents = new int[defaultSizeBuffer];
            indexesComponents = new int[defaultSizeBuffer];
            for (int i = 0; i < typesComponents.Length; i++)
            {
                typesComponents[i] = -1;
                indexesComponents[i] = -1;
            }
            IsAlive = true;
            container = ComponentContainer.GetInstance();
            this.IndexEntity = indexEntity;
            this.mainClass = mainClass;

            firstIndex = 0;
            lastIndex = 0;
        }

        internal Entity ActivateEntity(int indexEntity)
        {
            //IndexesComponents = new int[defaultSizeBuffer * 2];
            for (int i = 0; i < typesComponents.Length; i++)
            {
                typesComponents[i] = -1;
                indexesComponents[i] = -1;
            }
            IsAlive = true;
            this.IndexEntity = indexEntity;

            firstIndex = 0;
            lastIndex = 0;

            return this;
        }

        public void DeleteEntity()
        {
            IsAlive = false;
            //Перебираем индексы типов структур и удаляем компоненты
            for (int i = 0; i < typesComponents.Length; i++)
            {
                if (typesComponents[i] >= 0)
                {
                    container.GetComponent(typesComponents[i]).DeleteComponent(indexesComponents[i]);
                }
            }
            if (IndexEntity < mainClass.firstFreeIndex)
            {
                mainClass.firstFreeIndex = IndexEntity;
            }
        }

        public ref T AddOrGetComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent();
            int HashType = componentClass.HashType;
            int indexNewComponents = -1;

            //Перебираем индексы типов структур
            for (int i = 0; i < typesComponents.Length; i++)
            {
                //Если структура уже прикреплена то возвращем ссылку на нее
                if (typesComponents[i] == HashType)
                {
                    return ref componentClass.GetComponent(indexesComponents[i], IndexEntity);
                }
                //Находим первый попавшийся свободный индекс
                if (typesComponents[i] == -1 && indexNewComponents == -1)
                {
                    indexNewComponents = i;
                }
            }
            //Если компонент не найден то создаем компонент на месте найденного индекса иначе расширяем контейнер
            if (indexNewComponents >= 0)
            {
                typesComponents[indexNewComponents] = HashType;

                if (indexNewComponents < firstIndex)
                {
                    firstIndex = indexNewComponents;
                }
                if (indexNewComponents > lastIndex)
                {
                    lastIndex = indexNewComponents;
                }

                return ref componentClass.AddComponent(out indexesComponents[indexNewComponents], IndexEntity);
            }
            else
            {
                int index = typesComponents.Length;
                Array.Resize(ref typesComponents, typesComponents.Length * 2);
                Array.Resize(ref indexesComponents, indexesComponents.Length * 2);
                typesComponents[index] = HashType;

                if (index < firstIndex)
                {
                    firstIndex = index;
                }
                if (index > lastIndex)
                {
                    lastIndex = index;
                }

                return ref componentClass.AddComponent(out indexesComponents[index], IndexEntity);

            }
        }

        //Удаление компонента по типу структуры
        public void DeleteComponent<T>() where T : struct
        {
            Component<T> componentClass = Component<T>.GetInstanceComponent();
            int HashType = componentClass.HashType;
            bool componentsExist = false;
            for (int i = 0; i < typesComponents.Length; i++)
            {
                if (typesComponents[i] == HashType)
                {
                    componentClass.DeleteComponent(in indexesComponents[i]);
                    typesComponents[i] = -1;
                    indexesComponents[i] = -1;

                    if (i == firstIndex && i < (typesComponents.Length - 1))
                    {
                        firstIndex++;
                    }
                    if (i == lastIndex && i > 0)
                    {
                        lastIndex--;
                    }
                }
                else if (typesComponents[i] >= 0)
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
            for (int i = firstIndex; i < (lastIndex + 1); i++)
            {
                if (typesComponents[i] == HashType)
                {
                    return true;
                }
            }
            return false;
        }

        

    }

}


