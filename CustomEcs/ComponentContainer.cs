using System.Collections.Generic;
namespace CustomEcs
{

    internal class ComponentContainer
    {
        private static ComponentContainer container;

        private MainClassECS mainClass;

        List<BaseComponent> componentsClass;

        private ComponentContainer(MainClassECS mainClass)
        {
            this.mainClass = mainClass;
            componentsClass = new List<BaseComponent>();
        }

        public static ComponentContainer GetInstance(MainClassECS mainClass)
        {
            if (container == null)
            {
                container = new ComponentContainer(mainClass);
            }
            return container;
        }

        public static ComponentContainer GetInstance()
        {
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

        public BaseComponent GetComponent(int hash)
        {
            foreach (BaseComponent item in componentsClass)
            {
                if(item.HashType == hash)
                {
                    return item;
                }
            }
            return null;
        }
    }



}

