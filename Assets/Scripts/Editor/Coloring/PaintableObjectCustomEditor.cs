using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PaintableObject))]
public class PaintableObjectCustomEditor : Editor
{
    SerializedProperty paintableGroup;
    SerializedProperty objectID;
    SerializedProperty color;
    SerializedProperty applyMode;
    SerializedProperty materialIndex;
    SerializedProperty OnPainted;

    private void OnEnable()
    {
        paintableGroup = serializedObject.FindProperty("paintableGroup");
        objectID = serializedObject.FindProperty("objectID");
        color = serializedObject.FindProperty("color");
        applyMode = serializedObject.FindProperty("applyMode");
        materialIndex = serializedObject.FindProperty("materialIndex");
        OnPainted = serializedObject.FindProperty("OnPainted");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(paintableGroup);

        // Header "Configuration"
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);

        // Show objectID only if paintableGroup is null
        if (paintableGroup.objectReferenceValue == null)
            EditorGUILayout.PropertyField(objectID);

        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(applyMode);

        // Show materialIndex only if applyMode is OneMaterial
        if ((PaintableObject.ApplyMode)applyMode.enumValueIndex == PaintableObject.ApplyMode.OneMaterial)
            EditorGUILayout.PropertyField(materialIndex);

        EditorGUILayout.PropertyField(OnPainted);

        serializedObject.ApplyModifiedProperties();
    }
}
