using System.Collections.Generic;
using Elements.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    public class Simulation : MonoBehaviour
    {
        public static float DeltaTime => Time.deltaTime * Config.m_deltaTimeModifier;

        public void Start()
        {
            ResizeGrid(2, 2);
            
            var width = m_grid.Count;
            
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < m_grid[x].Count; ++y)
                {
                    m_grid[x][y].AddRenderer();
                }
            }
        }
        
        public void Update()
        {
            var width = m_grid.Count;
            
            // Simulate each grid space individually
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < m_grid[x].Count; ++y)
                {
                    m_grid[x][y].Update();
                }
            }
            
            // NOTE: The math for spreading right now will have issues because it's being done in many steps,
            // which each step affecting the one after it. Order matters right now and ideally it shouldn't.
            
            // Simulate interactions between adjacent grid spaces (no diagonal spread)
            for (int x = 0; x < width; ++x)
            {
                var height = m_grid[x].Count;
                
                for (int y = 0; y < height; ++y)
                {
                    var gridSpace = m_grid[x][y];
                    
                    // Above
                    if (y > 0)
                    {
                        gridSpace.UpdateSpread(m_grid[x][y - 1]);
                    }
                    
                    // Below
                    if (y < height - 1)
                    {
                        gridSpace.UpdateSpread(m_grid[x][y + 1]);
                    }
                    
                    // Left
                    if (x > 0)
                    {
                        gridSpace.UpdateSpread(m_grid[x - 1][y]);
                    }
                    
                    
                    // Right
                    if (x < width - 1)
                    {
                        gridSpace.UpdateSpread(m_grid[x + 1][y]);
                    }
                }
            }

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < m_grid[x].Count; ++y)
                {
                    m_grid[x][y].HandleDestroyStatus();
                }
            }

            HandleInput();
        }

        public void ResizeGrid(int width, int height)
        {
            m_grid.Resize(width, null);
            
            for (int x = 0; x < width; ++x)
            {
                if (m_grid[x] == null)
                {
                    m_grid[x] = new List<GridSpace>(height);
                }
                
                m_grid[x].Resize(height, null);
                
                for (int y = 0; y < height; ++y)
                {
                    if (m_grid[x][y] == null)
                    {
                        m_grid[x][y] = new GridSpace(this, x, y);
                    }
                }
            }
        }

        private void HandleInput()
        {
            
        }

        private readonly List<List<GridSpace>> m_grid = new List<List<GridSpace>>();
    }

    [System.Serializable]
    public class GridSpace
    {
        public GridSpace(Simulation simulation, int xPos, int yPos)
        {
            m_object = new GameObject(string.Concat("gridSpace_", xPos.ToString(), "_", yPos.ToString()));
            m_object.transform.SetParent(simulation.transform);
            
            m_object.transform.SetPositionAndRotation(new Vector3(xPos, yPos, 0.0f), Quaternion.identity);
        }
        
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
                component.UpdateChemistry(this);
            }

            HandleDestroyStatus();
        }

        public void UpdateSpread(GridSpace other)
        {
            foreach (var component in m_components)
            {
                component.Value.UpdateSpread(other);
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

        public TComponent AddType<TComponent>() where TComponent : Component
        {
            var component = m_object.GetComponent<TComponent>();
            if (component == null)
            {
                m_object.AddComponent<TComponent>();
                component = m_object.GetComponent<TComponent>();
            }

            return component;
        }

        public void AddRenderer()
        {
            var gridObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GridSpace"));           
            gridObj.transform.SetParent(m_object.transform, false);
            
            var renderer = gridObj.GetComponent<ComponentRenderer>();
            renderer.InitializeSpace(this);
        }
        
        public GameObject m_object;

        public Dictionary<ComponentType, Component> m_components = new Dictionary<ComponentType, Component>();
    }
}