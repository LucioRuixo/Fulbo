using UnityEditor;
using UnityEngine;

namespace Fulbo.CustomEditors
{
    using Match;

    [CustomEditor(typeof(Pitch))]
    public class PitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Update")) (target as Pitch).Initialize(null);
        }
    }
}