#if UNITY_EDITOR
using UnityEditor;

namespace Elements
{
    [CustomEditor(typeof(Simulation))]
    public class SimulationInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            Simulation simulation = (Simulation)target;
            
            int newWidth = EditorGUILayout.DelayedIntField("Grid Width", simulation.m_gridWidth);
            int newHeight = EditorGUILayout.DelayedIntField("Grid Height", simulation.m_gridHeight);

            bool changed = (newWidth != simulation.m_gridWidth);
            changed |= (newHeight != simulation.m_gridHeight);
            
            if (changed)
            {
                simulation.ResizeGrid(newWidth, newHeight);
            }
        }
    }
}
#endif // UNITY_EDITOR