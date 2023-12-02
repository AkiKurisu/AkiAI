using System.Reflection;
using Kurisu.GOAP;
using Kurisu.GOAP.Editor;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiAI.Editor
{
    [CustomEditor(typeof(AIAgent), true)]
    public class AIAgentEditor : UnityEditor.Editor
    {
        private static Color AkiBlue = new(140 / 255f, 160 / 255f, 250 / 255f);
        private AIAgent Agent => target as AIAgent;
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("AkiAI Agent", new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter }, GUILayout.MinHeight(30));
            GUILayout.Label("AI Status       " + (Agent.enabled ?
             (Agent.IsAIEnabled ? "<color=#92F2FF>Running</color>" : "<color=#FFF892>Pending</color>")
             : "<color=#FF787E>Disabled</color>"), new GUIStyle(GUI.skin.label) { richText = true });
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
            if (!Application.isPlaying)
            {
                var color = GUI.backgroundColor;
                GUI.backgroundColor = AkiBlue;
                if (GUILayout.Button("Edit GOAP Set"))
                {
                    GOAPEditorWindow.ShowEditorWindow((IGOAPSet)typeof(AIAgent).GetField("dataSet", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(target));
                }
                GUI.backgroundColor = color;
            }
        }
    }
}
