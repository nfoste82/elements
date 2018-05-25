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
        public void UpdateChemistry(GridSpace selfSpace)
        {
            InternalUpdateChemistry(selfSpace);   
            
            m_amountRemaining = Mathf.Clamp(m_amountRemaining, 0.0f, 1.0f);

            if (m_amountRemaining == 0.0f)
            {
                FlagForDestroying();
            }
        }

        public void UpdateSpread(GridSpace otherSpace)
        {
            InternalUpdateSpread(otherSpace);
            
            m_amountRemaining = Mathf.Clamp(m_amountRemaining, 0.0f, 1.0f);

            if (m_amountRemaining == 0.0f)
            {
                FlagForDestroying();
            }
        }

        protected virtual void InternalUpdateChemistry(GridSpace selfSpace)
        {
        }

        protected virtual void InternalUpdateSpread(GridSpace otherSpace)
        {
        }

        public void FlagForDestroying()
        {
            m_needsDestroying = true;
        }

        public ComponentType ComponentType { get; protected set; }

        public bool m_needsDestroying;
        public float m_amountRemaining = 1.0f;
    }
}