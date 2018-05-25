using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Air : Element
    {
        protected override void InternalUpdateChemistry(Dictionary<ComponentType, Component> interactions)
        {
            // Fire burns some of the air
            if (interactions.ContainsKey(ComponentType.Fire))
            {
                Debug.Log("Air amount reduced by fire burning through it.");
                
                // TODO: Could reduce the amount of air based on the amount of fire.
                
                m_amountRemaining -= Time.deltaTime * 0.2f;
            }
        }
    }
}