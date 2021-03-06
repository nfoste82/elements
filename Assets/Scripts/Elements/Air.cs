﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Air : Element
    {
        protected override void InternalUpdateChemistry(GridSpace selfSpace)
        {
            var interactions = selfSpace.m_components;
            
            // Fire burns some of the air
            if (interactions.ContainsKey(ComponentType.Fire))
            {
                Debug.Log("Air amount reduced by fire burning through it.");
                
                // TODO: Could reduce the amount of air based on the amount of fire.
                
                m_amountRemaining -= Simulation.DeltaTime * 0.2f;
            }
        }

        protected override void InternalUpdateSpread(GridSpace otherSpace)
        {
            // Air -> Air spread
            Component air;
            if (!otherSpace.m_components.TryGetValue(ComponentType.Air, out air))
            {
                // Adjacent space has no air so we need to create some
                air = otherSpace.AddType<Air>();
                air.m_amountRemaining = 0.0f;
            }
            
            float difference = m_amountRemaining - air.m_amountRemaining;
                
            // If the two spaces are close to a pressure equilibrium then don't bother doing anything.
            if (Mathf.Abs(difference) > 0.01f)
            {
                float average = (m_amountRemaining + air.m_amountRemaining) * 0.5f;
                
                // Our air increases/decreases based on the adjacent space.
                float oldAmount = m_amountRemaining;
                float newAmount = Mathf.Lerp(m_amountRemaining, average, Simulation.DeltaTime * 0.25f);
                    
                float delta = (newAmount - oldAmount);
                    
                m_amountRemaining += delta;
                    
                // The delta moves to/from the adjacent space
                air.m_amountRemaining -= delta;
            }
        }

        public void Start()
        {
            ComponentType = ComponentType.Air;
        }
    }
}