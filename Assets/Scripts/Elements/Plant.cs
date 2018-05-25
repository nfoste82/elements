using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Plant : Element
    {
        protected override void InternalUpdateChemistry(Dictionary<ComponentType, Component> interactions)
        {
            // Fire burns some of the plant
            if (interactions.ContainsKey(ComponentType.Fire))
            {
                Debug.Log("Plant amount reduced by fire burning it.");
                
                // TODO: Could reduce the amount of air based on the amount of fire.
                
                // TODO: Burn rate of plant could depend on the type of plant
                
                m_amountRemaining -= Time.deltaTime * 0.1f;
            }

            // If there's no air present the plant suffocates slowly
            if (!interactions.ContainsKey(ComponentType.Air))
            {
                Debug.Log("Plant is suffocating due to lack of air.");
                
                m_amountRemaining -= Time.deltaTime * 0.03f;
            }
            else
            {
                if (interactions.ContainsKey(ComponentType.Water))
                {
                    Debug.Log("Plant growing because water and air are both present.");
                    
                    // TODO: Could also require light to grow
                    
                    m_amountRemaining += Time.deltaTime * 0.005f;
                }
                else
                {
                    Debug.Log("Plant wilting because water is not present.");
                    
                    m_amountRemaining -= Time.deltaTime * 0.001f;
                }
            }
        }
        
        public void Start()
        {
            ComponentType = ComponentType.Plant;
        }
    }
}