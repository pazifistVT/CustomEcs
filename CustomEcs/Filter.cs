using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters;

namespace CustomEcs
{
    public enum TypeFilter
    {
        inc,
        ex
    }

    public abstract class BaseFilter
    {
        static readonly int defaultSizeFilter = 128;
        internal abstract void UpdateFilter();
        internal MainClassECS mainClass;
        
        internal CollectionEnumenator collectionEnumenator;
        public IEnumerator<int> GetEnumerator() => collectionEnumenator;

        //Получение сущности по ее индексу(индекс необходимо брать из контейнера фильтра)
        public Entity GetEntity(int index)
        {
            return mainClass.entities[index];
        }

        internal class CollectionEnumenator : IEnumerator<int>
        {
            internal int[] FilteredEntities;//Контейнер индексы сущностей отвечающих критериям фильтра
            private int position;
            private int lastIndex;
            internal int LastIndex
            {
                get
                {
                    return lastIndex;
                }
                set
                {
                    lastIndex = value;
                    if(LastIndex > (FilteredEntities.Length - 1))
                    {
                        Array.Resize<int>(ref FilteredEntities, (FilteredEntities.Length * 2));
                    }
                }
            }
            public int Current
            {
                get
                {
                    return FilteredEntities[position];
                }
            }

            object IEnumerator.Current => Current;

            public CollectionEnumenator()
            {
                lastIndex = 0;
                position = -1;
                FilteredEntities = new int[defaultSizeFilter];
            }

            public void Dispose()
            {
                position = -1;
            }

            public bool MoveNext()
            {
                if ((position < (FilteredEntities.Length - 1))
                    && (position < lastIndex))
                {
                    position++;
                    return true;
                }
                else
                    return false;
            }

            public void Reset()
            {
                position = -1;
            }
        }

        internal void AddNewElement(int IndexEntity)
        {
            collectionEnumenator.LastIndex++;
            collectionEnumenator.FilteredEntities[collectionEnumenator.LastIndex] = IndexEntity;
        }
    }
    public class Filter<T> : BaseFilter where T : struct
    {
        static Filter<T> obj;
        private static readonly int HashType = Component<T>.GetInstanceComponent().HashType;

        public static Filter<T> GetFilter()
        {
            if (obj == null)
            {
                obj = new Filter<T>
                {
                    collectionEnumenator = new CollectionEnumenator()
                };
            }
            return obj;
        }

        //Обновление фильтра - поиск сущностей имеющих структуры заданного типа
        internal override void UpdateFilter()
        {
            collectionEnumenator.LastIndex = -1;

            for (int i = 0; i < mainClass.entities.Length; i++)
            {
                if (mainClass.entities[i] != null && mainClass.entities[i].IsAlive) 
                {
                    if(mainClass.entities[i].CheckComponentType(HashType))
                    {
                        AddNewElement(mainClass.entities[i].IndexEntity);
                    }
                }
            }
        }

        //Получение конмонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref T GetComponent(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<T>();
        }
    }

    public class Filter<T, U> : BaseFilter where T : struct where U : struct
    {
        private static readonly int HashTypeT = Component<T>.GetInstanceComponent().HashType;
        private static readonly int HashTypeU = Component<U>.GetInstanceComponent().HashType;
        private bool typeT;
        private bool typeU;

        private static Filter<T, U> obj;
        public static Filter<T, U> GetFilter(TypeFilter T_, TypeFilter U_)
        {
            if (obj == null)
            {
                obj = new Filter<T, U>
                {
                    typeT = (T_ == TypeFilter.inc),
                    typeU = (U_ == TypeFilter.inc),
                    collectionEnumenator = new CollectionEnumenator()
                };
            }

            return obj;
        }

        //Обновление фильтра - поиск сущностей имеющих структуры заданного типа
        internal override void UpdateFilter()
        {
            collectionEnumenator.LastIndex = -1;

            for (int i = 0; i < mainClass.entities.Length; i++)
            {
                if (mainClass.entities[i] != null && mainClass.entities[i].IsAlive)
                {
                    if ((mainClass.entities[i].CheckComponentType(HashTypeT) && typeT) && (mainClass.entities[i].CheckComponentType(HashTypeU) && typeU) )
                    {
                        AddNewElement(mainClass.entities[i].IndexEntity);
                    }
                }
            }
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref T GetComponent1(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<T>();
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref U GetComponent2(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<U>();
        }
    }

    public class Filter<T, U, Y> : BaseFilter where T : struct where U : struct where Y : struct
    {
        private static readonly int HashTypeT = Component<T>.GetInstanceComponent().HashType;
        private static readonly int HashTypeU = Component<U>.GetInstanceComponent().HashType;
        private static readonly int HashTypeY = Component<Y>.GetInstanceComponent().HashType;
        private bool typeT;
        private bool typeU;
        private bool typeY;

        private static Filter<T, U, Y> obj;
        public static Filter<T, U, Y> GetFilter(TypeFilter T_, TypeFilter U_, TypeFilter Y_)
        {
            if (obj == null)
            {
                obj = new Filter<T, U, Y>
                {
                    typeT = (T_ == TypeFilter.inc),
                    typeU = (U_ == TypeFilter.inc),
                    typeY = (Y_ == TypeFilter.inc),
                    collectionEnumenator = new CollectionEnumenator()
                };
            }
            return obj;
        }

        //Обновление фильтра - поиск сущностей имеющих структуры заданного типа
        internal override void UpdateFilter()
        {
            collectionEnumenator.LastIndex = -1;


            for (int i = 0; i < mainClass.entities.Length; i++)
            {
                if (mainClass.entities[i] != null && mainClass.entities[i].IsAlive)
                {
                    if ((mainClass.entities[i].CheckComponentType(HashTypeT) && typeT) && 
                        (mainClass.entities[i].CheckComponentType(HashTypeU) && typeU) &&
                        (mainClass.entities[i].CheckComponentType(HashTypeY)) && typeY)
                    {
                        AddNewElement(mainClass.entities[i].IndexEntity);
                    }
                }
            }
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref T GetComponent1(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<T>();
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref U GetComponent2(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<U>();
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref Y GetComponent3(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<Y>();
        }
    }

    public class Filter<T, U, Y, I> : BaseFilter where T : struct where U : struct where Y : struct where I : struct
    {
        private static readonly int HashTypeT = Component<T>.GetInstanceComponent().HashType;
        private static readonly int HashTypeU = Component<U>.GetInstanceComponent().HashType;
        private static readonly int HashTypeY = Component<Y>.GetInstanceComponent().HashType;
        private static readonly int HashTypeI = Component<I>.GetInstanceComponent().HashType;

        private bool typeT;
        private bool typeU;
        private bool typeY;
        private bool typeI;

        private static Filter<T, U, Y, I> obj;
        public static Filter<T, U, Y, I> GetFilter(TypeFilter T_, TypeFilter U_, TypeFilter Y_, TypeFilter I_)
        {
            if (obj == null)
            {
                obj = new Filter<T, U, Y, I>
                {
                    typeT = (T_ == TypeFilter.inc),
                    typeU = (U_ == TypeFilter.inc),
                    typeY = (Y_ == TypeFilter.inc),
                    typeI = (I_ == TypeFilter.inc),
                    collectionEnumenator = new CollectionEnumenator()
                };
            }
            return obj;
        }

        //Обновление фильтра - поиск сущностей имеющих структуры заданного типа
        internal override void UpdateFilter()
        {
            collectionEnumenator.LastIndex = -1;


            for (int i = 0; i < mainClass.entities.Length; i++)
            {
                if (mainClass.entities[i] != null && mainClass.entities[i].IsAlive)
                {
                    if ((mainClass.entities[i].CheckComponentType(HashTypeT) && typeT) &&
                        (mainClass.entities[i].CheckComponentType(HashTypeU) && typeU) &&
                        (mainClass.entities[i].CheckComponentType(HashTypeY) && typeY) &&
                        (mainClass.entities[i].CheckComponentType(HashTypeI)) && typeI)
                    {
                        AddNewElement(mainClass.entities[i].IndexEntity);
                    }
                }
            }
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref T GetComponent1(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<T>();
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref U GetComponent2(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<U>();
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref Y GetComponent3(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<Y>();
        }

        //Получение конпонента по индексу его сущности(индекс необходимо брать из контейнера фильтра)
        public ref I GetComponent4(int index)
        {
            return ref mainClass.entities[index].AddOrGetComponent<I>();
        }
    }
}
