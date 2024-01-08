using System;
using System.Runtime.Serialization.Formatters;

namespace CustomEcs
{
    abstract class BaseComponent
    {
        internal int HashType { get; set; }

        protected bool[] aliveComponents;//признак жизни компонента
        protected internal int[] indexesEntity;//индекс сущности которой принадлежит компонент
        protected internal int lastDeleteComp;//индекс последнего удаленного компонента
        protected internal int lastCreatedComp;//индекс последнего созданного компонента
        //Удаление структуры по указанному индексу
        internal void DeleteComponent(in int index)
        {
            aliveComponents[index] = false;
            lastDeleteComp = index;
            if(lastCreatedComp > lastDeleteComp)
            {
                lastCreatedComp = lastDeleteComp;
            }
        }
    }

    internal class Component<T> : BaseComponent where T : struct
    {
        static readonly int defaultSizeBuffer = 32;
        static Component<T> obj;

        T[] components;//контейнер структур хранящих данные
        
        public static Component<T> GetInstanceComponent()
        {
            if (obj == null)
            {
                obj = new Component<T>();
                obj.components = new T[defaultSizeBuffer];
                obj.aliveComponents = new bool[defaultSizeBuffer];
                obj.indexesEntity = new int[defaultSizeBuffer];
                obj.lastDeleteComp = 0;
                obj.lastCreatedComp = 0;
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
            lastCreatedComp = index;
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
            if (!aliveComponents[lastDeleteComp])
            {
                indexNewComponent = lastDeleteComp;
                return ref CreateNewComponent(lastDeleteComp, entityIndex);
            }
            for (int i = (lastCreatedComp + 1); i < aliveComponents.Length; i++)
            {
                if (!aliveComponents[i])
                {
                    indexNewComponent = i;
                    return ref CreateNewComponent(i, entityIndex);
                }
            }
            Array.Resize(ref aliveComponents, aliveComponents.Length * 2);
            Array.Resize(ref components, components.Length * 2);
            Array.Resize(ref indexesEntity, components.Length * 2);
            return ref CreateNewComponent(indexNewComponent, entityIndex);
        }
        
    }
}

