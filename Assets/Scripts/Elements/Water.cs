using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Water : Element
    {
        protected override void InternalUpdateChemistry(GridSpace selfSpace)
        {
            var interactions = selfSpace.m_components;
            
            if (interactions.ContainsKey(ComponentType.Plant))
            {
                // Water is consumed by plants
                m_amountRemaining -= Time.deltaTime * 0.0001f;
            }
        }
        
        protected override void InternalUpdateSpread(GridSpace otherSpace)
        {
            // Water -> Water spread
            Component water;
            if (!otherSpace.m_components.TryGetValue(ComponentType.Water, out water))
            {
                // Adjacent space has no air so we need to create some
                water = otherSpace.AddType<Water>();
                water.m_amountRemaining = 0.0f;
            }
            
            float difference = m_amountRemaining - water.m_amountRemaining;
                
            // If the two spaces are close to a pressure equilibrium then don't bother doing anything.
            if (Mathf.Abs(difference) > 0.01f)
            {
                float average = (m_amountRemaining + water.m_amountRemaining) * 0.5f;
                
                // Water increases/decreases based on the adjacent space.
                float oldAmount = m_amountRemaining;
                float newAmount = Mathf.Lerp(m_amountRemaining, average, Time.deltaTime * 0.25f);
                    
                float delta = (newAmount - oldAmount);
                    
                m_amountRemaining += delta;
                    
                // The delta moves to/from the adjacent space
                water.m_amountRemaining -= delta;
            }
        }
        
        public void Start()
        {
            ComponentType = ComponentType.Water;
        }
    }
}