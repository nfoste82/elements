using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Fire : Element
    {
        protected override void InternalUpdateChemistry(Dictionary<ComponentType, Component> interactions)
        {
            // If water is present the fire is put out
            if (interactions.ContainsKey(ComponentType.Water))
            {
                Debug.Log("Fire put out by water.");
                m_amountRemaining = 0.0f;
                return;
            }
            
            // If we have no air then we reduce the fire strength
            if (!interactions.ContainsKey(ComponentType.Air))
            {
                Debug.Log("Fire weakening due to lack of air.");
                m_amountRemaining -= Time.deltaTime * 0.2f;
            }            
            
            // If we have no plant material (fuel) then we reduce the fire strength. If fuel
            // is present then we move the strength closer to full strength
            if (interactions.ContainsKey(ComponentType.Plant))
            {
                Debug.Log("Fire continuing to burn fuel.");
                m_amountRemaining += Time.deltaTime * 0.1f;
            }
            else
            {
                Debug.Log("Fire weakening due to lack of fuel.");
                m_amountRemaining -= Time.deltaTime * 0.2f;    
            }
        }
        
        public void Start()
        {
            ComponentType = ComponentType.Fire;
        }
    }
}