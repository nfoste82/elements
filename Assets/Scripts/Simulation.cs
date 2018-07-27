using System.Collections.Generic;
using Elements.Extensions;
using UnityEngine;

namespace Elements
{
    public class Simulation : MonoBehaviour
    {
        public static float DeltaTime => Time.deltaTime * Config.m_deltaTimeModifier;

        public void Start()
        {
            ResizeGrid(2, 2);
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
        }

        public void ResizeGrid(int width, int height)
        {
            // Width has gotten smaller, we need to delete grid spaces
            if (width < m_gridWidth)
            {
                for (int x = width; x < m_gridWidth; ++x)
                {
                    foreach (var space in m_grid[x])
                    {
                        Destroy(space.m_object); 
                    }
                }
            }

            // Height has gotten smaller, we need to delete grid spaces
            if (height < m_gridHeight)
            {
                for (int x = width; x < m_grid.Count; ++x)
                {
                    for (int y = height; y < m_gridHeight; ++y)
                    {
                        Destroy(m_grid[x][y].m_object);
                    }
                }
            }
            
            m_gridWidth = width;
            m_gridHeight = height;
            
            
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
                        var space = new GridSpace(this, x, y);
                        m_grid[x][y] = space;
                        
                        space.Initialize();
                    }
                }
            }
        }

        private readonly List<List<GridSpace>> m_grid = new List<List<GridSpace>>();

        public int m_gridWidth = 1;
        public int m_gridHeight = 1;
    }
}