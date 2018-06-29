using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Elements
{
    public class ComponentRenderer : MonoBehaviour
    {
        private Image Background => gameObject.transform.FindDeepChild("Background").GetComponent<Image>();
        private Image Plant => gameObject.transform.FindDeepChild("Plant").GetComponent<Image>();
        private Image Fire => gameObject.transform.FindDeepChild("Fire").GetComponent<Image>();
        private Image Smoke => gameObject.transform.FindDeepChild("Smoke").GetComponent<Image>();

        private GridSpace m_space;
        private readonly Dictionary<ComponentType, float> m_amountsRemaining = new Dictionary<ComponentType, float>();

        public void InitializeSpace(GridSpace space)
        {
            if (m_space != null)
            {
                throw new System.ApplicationException("Space already initialized: " + m_space.m_object.name);
            }
            
            m_space = space;
        }

        public void Update()
        {
            foreach (var kvp in m_space.m_components)
            {
                float amount;
                if (m_amountsRemaining.TryGetValue(kvp.Key, out amount))
                {
                    // If the difference is large enough to warrant an update
                    if (Math.Abs(amount - kvp.Value.m_amountRemaining) > 0.02)
                    {
                        m_amountsRemaining[kvp.Key] = kvp.Value.m_amountRemaining;
                        UpdateRenderForComponentType(kvp.Key, amount, kvp.Value.m_amountRemaining);
                    }
                }
                else
                {
                    m_amountsRemaining[kvp.Key] = kvp.Value.m_amountRemaining;
                    UpdateRenderForComponentType(kvp.Key, 0.0f, kvp.Value.m_amountRemaining);
                }
            }

            var removedComponents = m_amountsRemaining.Keys.Except(m_space.m_components.Keys).ToList();

            foreach (var compType in removedComponents)
            {
                UpdateRenderForComponentType(compType, m_amountsRemaining[compType], 0.0f);
                
                m_amountsRemaining.Remove(compType);
            }
        }

        private void UpdateRenderForComponentType(ComponentType componentType, float oldValue, float newValue)
        {
            switch (componentType)
            {
                case ComponentType.Water:
                    Background.color = new Color(Mathf.Lerp(255.0f/255.0f, 0.0f/255.0f, newValue),
                        Mathf.Lerp(255.0f/255.0f, 164.0f/255.0f, newValue),
                        1.0f);
                    break;
                case ComponentType.Plant:
                {
                    var plant = Plant;
                    
                    plant.gameObject.SetActive(newValue != 0.0f);    
                    
                    var scale = Mathf.Lerp(0.1f, 1.0f, newValue);
                    plant.transform.localScale = new Vector3(scale, scale, 1.0f);
                }
                    break;
                case ComponentType.Fire:
                {
                    var fire = Fire;
                    
                    fire.gameObject.SetActive(newValue != 0.0f);
                    
                    var scale = Mathf.Lerp(0.1f, 1.0f, newValue);
                    fire.transform.localScale = new Vector3(scale, scale, 1.0f);
                    
                    // If there's plant and air around then we need smoke
                    bool hasSmoke = m_amountsRemaining.ContainsKey(ComponentType.Air) && m_amountsRemaining.ContainsKey(ComponentType.Plant);
                    
                    var smoke = Smoke;                        
                    smoke.gameObject.SetActive(hasSmoke && newValue != 0.0f);
                    
                    var smokeScale = Mathf.Lerp(0.3f, 1.0f, newValue);
                    smoke.transform.localScale = new Vector3(smokeScale, smokeScale, 1.0f);
                }
                    break;
            }
        }
    }
}