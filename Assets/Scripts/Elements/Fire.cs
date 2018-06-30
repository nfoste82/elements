using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Fire : Element
    {
        protected override void InternalUpdateChemistry(GridSpace selfSpace)
        {
            var interactions = selfSpace.m_components;
            
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
                m_amountRemaining -= Simulation.DeltaTime * 0.2f;
            }            
            
            // If we have no plant material (fuel) then we reduce the fire strength. If fuel
            // is present then we move the strength closer to full strength
            if (interactions.ContainsKey(ComponentType.Plant))
            {
                Debug.Log("Fire continuing to burn fuel.");
                m_amountRemaining += Simulation.DeltaTime * 0.1f;
            }
            else
            {
                Debug.Log("Fire weakening due to lack of fuel.");
                m_amountRemaining -= Simulation.DeltaTime * 0.2f;    
            }
        }
        
        protected override void InternalUpdateSpread(GridSpace otherSpace)
        {
            // Fire spread (a stronger fire will strengthen nearby weaker ones, and vice versa)
            //     This also handles the possibility of fire spreading to adjacent spaces
            FireSpread(otherSpace);
        }

        private void FireToNewAreaSpread(GridSpace otherSpace)
        {
            // Random chance to spread (based on strength of fire)
            float roll = Random.Range(0.0f, m_amountRemaining);
            
            // 25% chance per second (if fire is full strength)
            float odds = 1 - 0.25f * Simulation.DeltaTime;

            // If we didn't beat the odds then the fire doesn't spread yet
            if (roll < odds)
            {
                return;
            }
            
            // Cannot be wet
            if (otherSpace.m_components.ContainsKey(ComponentType.Water))
            {
                return;
            }
            
            // Must have plant (fuel)
            if (!otherSpace.m_components.ContainsKey(ComponentType.Plant))
            {
                return;
            }
            
            // Must have enough air to spread
            Component air;
            if (!otherSpace.m_components.TryGetValue(ComponentType.Air, out air) || air.m_amountRemaining < 0.1f)
            {
                return;
            }

            Debug.LogWarning("Fire has spread to " + otherSpace.m_object.name);
            
            var fire = otherSpace.AddType<Fire>();
            fire.m_amountRemaining = 0.05f;    // Start new fire out small
        }

        private void FireSpread(GridSpace otherSpace)
        {
            Component fire;
            if (!otherSpace.m_components.TryGetValue(ComponentType.Fire, out fire))
            {
                FireToNewAreaSpread(otherSpace);
                return;
            }
            
            float difference = m_amountRemaining - fire.m_amountRemaining;
                
            // If the two spaces are close to a fire equilibrium (heat?) then don't bother doing anything.
            if (Mathf.Abs(difference) > 0.01f)
            {
                float average = (m_amountRemaining + fire.m_amountRemaining) * 0.5f;
                
                float oldAmount = m_amountRemaining;
                float newAmount = Mathf.Lerp(m_amountRemaining, average, Simulation.DeltaTime * 0.05f);
                    
                float delta = (newAmount - oldAmount);
                    
                fire.m_amountRemaining += delta;
            }   
        }
        
        public void Start()
        {
            ComponentType = ComponentType.Fire;
        }
    }
}