﻿using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Simulation : MonoBehaviour
    {
        public void Update()
        {
            // Simulate each grid space individually
            for (int i = 0; i < m_grid.Length; ++i)
            {
                for (int j = 0; j < m_grid[i].m_row.Length; ++j)
                {
                    m_grid[i].m_row[j].Update();
                }
            }
            
            // NOTE: The math for spreading right now will have issues because it's being done in many steps,
            // which each step affecting the one after it. Order matters right now and ideally it shouldn't.
            
            // Simulate interactions between adjacent grid spaces (no diagonal spread)
            for (int i = 0; i < m_grid.Length; ++i)
            {
                for (int j = 0; j < m_grid[i].m_row.Length; ++j)
                {
                    // Above
                    if (i > 0)
                    {
                        m_grid[i].m_row[j].UpdateSpread(m_grid[i - 1].m_row[j]);
                    }
                    
                    // Below
                    if (i < m_grid.Length - 1)
                    {
                        m_grid[i].m_row[j].UpdateSpread(m_grid[i + 1].m_row[j]);
                    }
                    
                    // Left
                    if (j > 0)
                    {
                        m_grid[i].m_row[j].UpdateSpread(m_grid[i].m_row[j - 1]);
                    }
                    
                    
                    // Right
                    if (j < m_grid[i].m_row.Length - 1)
                    {
                        m_grid[i].m_row[j].UpdateSpread(m_grid[i].m_row[j + 1]);
                    }
                }
            }

            for (int i = 0; i < m_grid.Length; ++i)
            {
                for (int j = 0; j < m_grid[i].m_row.Length; ++j)
                {
                    m_grid[i].m_row[j].HandleDestroyStatus();
                }
            }
        }

        public GridRow[] m_grid;
    }

    [System.Serializable]
    public struct GridRow
    {
        public GridSpace[] m_row;
    }

    [System.Serializable]
    public class GridSpace
    {
        public void Update()
        {
            var components = m_object.GetComponents<Component>();

            // TODO: Obviously this is inefficient. Could optimize this or use something like Entitas instead.
            m_components.Clear();

            foreach (var component in components)
            {
                m_components.Add(component.ComponentType, component);
            }

            foreach (var component in components)
            {
                component.UpdateChemistry(m_components);
            }

            HandleDestroyStatus();
        }

        public void UpdateSpread(GridSpace other)
        {
            foreach (var component in m_components)
            {
                component.Value.UpdateSpread(other.m_components);
            }
        }

        public void HandleDestroyStatus()
        {
            List<ComponentType> destroyedTypes = null;
            
            foreach (var component in m_components)
            {
                if (component.Value.m_needsDestroying)
                {
                    if (destroyedTypes == null)
                    {
                        destroyedTypes = new List<ComponentType>();
                    }

                    destroyedTypes.Add(component.Key);
                }
            }

            if (destroyedTypes != null)
            {
                foreach (var componentType in destroyedTypes)
                {
                    Debug.LogWarning("Component destroyed: " + componentType + ", " + m_object.name);
                    GameObject.Destroy(m_components[componentType]);

                    m_components.Remove(componentType);
                }
            }
        }
        
        [SerializeField]
        private GameObject m_object;

        public Dictionary<ComponentType, Component> m_components = new Dictionary<ComponentType, Component>();
    }
}