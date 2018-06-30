using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Plant : Element
    {
        protected override void InternalUpdateChemistry(GridSpace selfSpace)
        {
            var interactions = selfSpace.m_components;
            
            // Fire burns some of the plant
            if (interactions.ContainsKey(ComponentType.Fire))
            {
                Debug.Log("Plant amount reduced by fire burning it.");
                
                // TODO: Could reduce the amount of air based on the amount of fire.
                
                // TODO: Burn rate of plant could depend on the type of plant
                
                m_amountRemaining -= Simulation.DeltaTime * 0.1f;
            }
            
            // Plants create air
            Component air = null;
            if (!interactions.TryGetValue(ComponentType.Air, out air))
            {
                // If no air exists then create some
                air = selfSpace.AddType<Air>();
            }

            // 1000 seconds to fill a grid space with air, if the plant is full size
            air.m_amountRemaining += Simulation.DeltaTime * 0.001f * m_amountRemaining;
            
            if (interactions.ContainsKey(ComponentType.Water))
            {
                Debug.Log("Plant growing because water is present.");
                
                // TODO: Could also require light to grow
                
                m_amountRemaining += Simulation.DeltaTime * 0.001f;
            }
            else
            {
                Debug.Log("Plant wilting because water is not present.");
                
                m_amountRemaining -= Simulation.DeltaTime * 0.0025f;
            }
        }
        
        public void Start()
        {
            ComponentType = ComponentType.Plant;
        }
    }
}