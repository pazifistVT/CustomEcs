using System;
using System.Collections.Generic;
namespace CustomEcs
{
    abstract class BaseComponent
    {
        public int HashType { get; set; }

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
            if (container == null)
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



}

