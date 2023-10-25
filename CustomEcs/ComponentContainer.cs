using System.Collections.Generic;
namespace CustomEcs
{
    abstract class BaseComponent
    {
        public int HashType { get; set; }
    }

    internal class ComponentContainer
    {
        private static ComponentContainer container;

        private MainClass mainClass;

        List<BaseComponent> componentsClass = new List<BaseComponent>();

        private ComponentContainer(MainClass mainClass)
        {
            this.mainClass = mainClass;
        }

        public static ComponentContainer GetInstance(MainClass mainClass)
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
    }



}

