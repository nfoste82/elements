using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Simulation : MonoBehaviour
    {
        public void Update()
        {
            var components = GetComponents<Component>();

            var componentDict = new Dictionary<ComponentType, Component>();

            foreach (var component in components)
            {
                componentDict.Add(component.m_componentType, component);
            }

            foreach (var component in components)
            {
                component.UpdateChemistry(componentDict);
            }

            foreach (var component in components)
            {
                if (component.m_needsDestroying)
                {
                    Debug.LogWarning("Component destroyed: " + component.m_componentType);
                    Destroy(component);
                }
            }
        }
    }
}