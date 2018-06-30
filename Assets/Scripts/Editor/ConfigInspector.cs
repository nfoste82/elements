﻿#if UNITY_EDITOR
using Elements;
using UnityEngine;
using UnityEditor;

namespace Elements
{
    [CustomEditor(typeof(Config))]
    public class ConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            Config.m_deltaTimeModifier = UnityEditor.EditorGUILayout.Slider("Delta Time Modifier", Config.m_deltaTimeModifier, 0.0f, 100.0f);
        }
    }
}
#endif // UNITY_EDITOR