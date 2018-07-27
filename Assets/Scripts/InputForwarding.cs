using System.Collections;
using System.Collections.Generic;
using Elements;
using UnityEngine;

public class InputForwarding : MonoBehaviour
{
    public GridSpace m_gridSpace;
    
    public void OnClicked()
    {
        m_gridSpace?.OnClicked();
    }
}
