using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public enum ComponentType
    {
        Fire,
        Water,
        Air,
        Earth,
        Stone,
        Metal,
        Plant
    }
    
    public class Component : MonoBehaviour
    {
        public void UpdateChemistry(Dictionary<ComponentType, Component> interactions)
        {
            InternalUpdateChemistry(interactions);   
            
            m_amountRemaining = Mathf.Clamp(m_amountRemaining, 0.0f, 1.0f);

            if (m_amountRemaining == 0.0f)
            {
                FlagForDestroying();
            }
        }

        protected virtual void InternalUpdateChemistry(Dictionary<ComponentType, Component> interactions)
        {
        }

        public void FlagForDestroying()
        {
            m_needsDestroying = true;
        }
        
        public ComponentType m_componentType;
        public bool m_needsDestroying;
        public float m_amountRemaining = 1.0f;
    }
}