using System.Collections.Generic;
using System.Text.Json;
using System;
namespace CustomEcs
{

    internal class ComponentContainer
    {
        private static ComponentContainer container;

        private MainClassECS mainClass;

        List<BaseComponent> componentsClass;

        internal string Serialize()
        {
            SerializeListComponents serializeListComponents = new SerializeListComponents();
            serializeListComponents.list = new List<SerializeComponent>();
            foreach (BaseComponent item in componentsClass)
            {
                SerializeComponent serializeComponent = new SerializeComponent();
                serializeComponent.componentType = item.HashType;
                serializeComponent.value = item.Serialize();
                serializeListComponents.list.Add(serializeComponent);
            }
            string s = JsonSerializer.Serialize<SerializeListComponents>(serializeListComponents);
            return s;
        }

        internal void Deserialize(string s)
        {
            SerializeListComponents entity = new SerializeListComponents();
            try
            {
                entity = JsonSerializer.Deserialize<SerializeListComponents>(s);
            }
            catch (Exception e)
            {

            }
            foreach (BaseComponent item in componentsClass)
            {
                item.Deserialize(entity);
            }
        }

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

