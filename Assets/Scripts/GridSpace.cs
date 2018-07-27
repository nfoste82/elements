using System.Collections.Generic;
using Elements;
using UnityEditor;
using UnityEngine;

public class GridSpace
{
    public GridSpace(Simulation simulation, int xPos, int yPos)
    {
        m_gridPosition = new Vector2Int(xPos, yPos);
        
        m_object = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GridSpace")); 
        m_object.name = string.Concat("gridSpace_", xPos.ToString(), "_", yPos.ToString());
        
        m_object.transform.SetParent(simulation.transform);
        
        m_object.transform.SetPositionAndRotation(new Vector3(xPos, yPos, 0.0f), Quaternion.identity);

        var inputComp = m_object.transform.FindDeepChild("Background").GetComponent<InputForwarding>();
        inputComp.m_gridSpace = this;
    }

    public void OnClicked()
    {
        Debug.LogWarning("Grid Space clicked: " + m_gridPosition.x + ", " + m_gridPosition.y);

        Selection.activeGameObject = m_object;
    }
    
    public void Update()
    {
        var components = m_object.GetComponents<Elements.Component>();

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

    public TComponent AddType<TComponent>() where TComponent : Elements.Component
    {
        var component = m_object.GetComponent<TComponent>();
        if (component == null)
        {
            m_object.AddComponent<TComponent>();
            component = m_object.GetComponent<TComponent>();
        }

        return component;
    }

    public void Initialize()
    {
        var renderer = m_object.GetComponent<ComponentRenderer>();
        renderer.InitializeSpace(this);
    }

    public Vector2Int m_gridPosition;
    
    public GameObject m_object;

    public Dictionary<ComponentType, Elements.Component> m_components = new Dictionary<ComponentType, Elements.Component>();
}
