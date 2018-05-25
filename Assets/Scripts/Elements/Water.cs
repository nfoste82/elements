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
        
        public void Start()
        {
            ComponentType = ComponentType.Water;
        }
    }
}