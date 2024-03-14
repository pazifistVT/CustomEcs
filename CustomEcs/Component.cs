using System;
using System.Text.Json;

namespace CustomEcs
{
    abstract class BaseComponent
    {
        internal int HashType { get; set; }

        protected internal bool[] aliveComponents;//признак жизни компонента
        protected internal int[] indexesEntity;//индекс сущности которой принадлежит компонент
        protected internal int firstFreeIndex;
        protected internal int lastFreeIndex;
        //Удаление структуры по указанному индексу
        internal void DeleteComponent(in int index)
        {
            aliveComponents[index] = false;
            if(index < firstFreeIndex)
            {
                firstFreeIndex = index;
            }
        }

        internal abstract void Deserialize(SerializeListComponents listComponents);
        internal abstract string Serialize();
    }

    internal class Component<T> : BaseComponent where T : struct
    {
        static readonly int defaultSizeBuffer = 32;
        static Component<T> obj;

        T[] components;//контейнер структур хранящих данные

        private class ComponentJson<T>
        {
            public T[] components;
            public bool[] aliveComponents;
            public int[] indexesEntity;
            public int firstFreeIndex;
            public int lastFreeIndex;
        }

        internal override string Serialize()
        {
            ComponentJson<T> entity = new ComponentJson<T>()
            {
                components = components,
                aliveComponents = aliveComponents,
                indexesEntity = indexesEntity,
                firstFreeIndex = firstFreeIndex,
                lastFreeIndex = lastFreeIndex
            };
            string s = JsonSerializer.Serialize<ComponentJson<T>>(entity);
            return s;
        }

        internal override void Deserialize(SerializeListComponents listComponents)
        {
            foreach (SerializeComponent item in listComponents.list)
            {
                if(item.componentType == HashType)
                {
                    ComponentJson<T> entity = new ComponentJson<T>();
                    try
                    {
                        entity = JsonSerializer.Deserialize<ComponentJson<T>>(item.value);
                    }
                    catch (Exception)
                    {

                    }
                    components = entity.components;
                    aliveComponents = entity.aliveComponents;
                    indexesEntity = entity.indexesEntity;
                    firstFreeIndex = entity.firstFreeIndex;
                    lastFreeIndex = entity.lastFreeIndex;
                }
            }
            
        }

        public static Component<T> GetInstanceComponent()
        {
            if (obj == null)
            {
                obj = new Component<T>();
                obj.components = new T[defaultSizeBuffer];
                obj.aliveComponents = new bool[defaultSizeBuffer];
                obj.indexesEntity = new int[defaultSizeBuffer];
                obj.firstFreeIndex = 0;
                obj.lastFreeIndex = defaultSizeBuffer;
                obj.HashType = obj.GetType().GetHashCode();

                ComponentContainer componentContainer = ComponentContainer.GetInstance();
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
        //Инициализация новой структуры в контейнере
        //index - индекс структуры внутри контейнера данных
        //entityIndex - индекс сущности которой принадлежит компонент    
        private ref T CreateNewComponent(int index, int entityIndex)
        {
            components[index] = new T();
            aliveComponents[index] = true;
            indexesEntity[index] = entityIndex;
            if(index == firstFreeIndex)
            {
                firstFreeIndex = (index + 1);
            }
            
            return ref components[index];
        }

        //Возврат структуры или создание новой если компонент по указанному индексу удален
        internal ref T GetComponent(in int index, int entityIndex)
        {
            if (aliveComponents[index])
            {
                return ref components[index];
            }
            else
            {
                return ref CreateNewComponent(index, entityIndex);
            }
        }

        //Создание новой структуры
        internal ref T AddComponent(out int indexNewComponent, int entityIndex)
        {
            indexNewComponent = aliveComponents.Length;
            //Если ранее происходило удаление компонента создаем структуру без перебора массива
            /*if (!aliveComponents[lastDeleteComp])
            {
                indexNewComponent = lastDeleteComp;
                return ref CreateNewComponent(lastDeleteComp, entityIndex);
            }*/
            for (int i = firstFreeIndex; i < lastFreeIndex; i++)
            {
                if (aliveComponents[i] == false)
                {
                    indexNewComponent = i;
                    return ref CreateNewComponent(i, entityIndex);
                }
            }
            Array.Resize(ref aliveComponents, aliveComponents.Length * 2);
            Array.Resize(ref components, components.Length * 2);
            Array.Resize(ref indexesEntity, indexesEntity.Length * 2);
            lastFreeIndex *= 2;
            return ref CreateNewComponent(indexNewComponent, entityIndex);
        }
        
    }
}

