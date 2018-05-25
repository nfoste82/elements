using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class Water : Element
    {
        protected override void InternalUpdateChemistry(Dictionary<ComponentType, Component> interactions)
        {
            if (interactions.ContainsKey(ComponentType.Plant))
            {
                // Plants consume water
                m_amountRemaining -= Time.deltaTime * 0.0001f;
            }
        }
    }
}