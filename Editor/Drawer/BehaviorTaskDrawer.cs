using Kurisu.AkiBT.Editor;
using UnityEditor;
using UnityEngine;
namespace Kurisu.AkiAI.Editor
{
    [CustomPropertyDrawer(typeof(BehaviorTask))]
    public class BehaviorTaskDrawer : PropertyDrawer
    {
        private static Color AkiGreen = new(170 / 255f, 255 / 255f, 97 / 255f);
        private static Color AkiBlue = new(140 / 255f, 160 / 255f, 250 / 255f);
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };
            var isPersistent = property.FindPropertyRelative("isPersistent");
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("isPersistent"));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            if (!isPersistent.boolValue)
            {
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("taskID"));
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("behaviorTree"));
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            GUI.enabled = Application.isPlaying;
            var task = ReflectionUtility.GetTargetObjectWithProperty(property) as BehaviorTask;
            var color = GUI.backgroundColor;
            GUI.backgroundColor = task.Enabled ? AkiGreen : AkiBlue;
            if (GUI.Button(rect, "Debug Task Behavior"))
            {
                GraphEditorWindow.Show(task.InstanceTree);
            }
            GUI.backgroundColor = color;
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            GUI.enabled = false;
            EditorGUI.Toggle(rect, "Task Enabled", task.Enabled);
            GUI.enabled = true;
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isPersistent = property.FindPropertyRelative("isPersistent").boolValue;
            return EditorGUIUtility.singleLineHeight * (isPersistent ? 4 : 5) + EditorGUIUtility.standardVerticalSpacing * (isPersistent ? 3 : 4);
        }
    }
}
