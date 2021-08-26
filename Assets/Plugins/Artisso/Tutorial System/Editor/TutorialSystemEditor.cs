using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditorInternal;
using Artisso.TutorialSystem;

[CustomEditor(typeof(TutorialSystem))]
public class TutorialSystemEditor : Editor
{
    TutorialSystem tutorial;
    bool showOptionalParameters = false;

    void OnEnable()
    {
        tutorial = (TutorialSystem)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        tutorial.autoStart = EditorGUILayout.Toggle(new GUIContent("Auto start", "Starts tutorial automatically. To start manually call StartTutorial method with tutorial number"), tutorial.autoStart);
        tutorial.tutorialNumber = EditorGUILayout.IntField(new GUIContent("Tutorial number", "Starts specific tutorial number"), tutorial.tutorialNumber);
        tutorial.defaultCanvas = (Canvas)EditorGUILayout.ObjectField(new GUIContent("Default canvas", "Default canvas tutorial system is attached to."), tutorial.defaultCanvas, typeof(Canvas), true);
        tutorial.dialogTemplate = (TutorialSystemDialogTemplate)EditorGUILayout.ObjectField(new GUIContent("Dialog Template", "Drop prefab of tutorial dialog template"), tutorial.dialogTemplate, typeof(TutorialSystemDialogTemplate), true);
       

        showOptionalParameters = EditorGUILayout.Foldout(showOptionalParameters, "Optional Parameters");

        if (showOptionalParameters)
        {
            tutorial.optionalParameters.allowToSkip = EditorGUILayout.Toggle(new GUIContent("Allow to skip", "If set to true shows close button in dialog window"), tutorial.optionalParameters.allowToSkip);
            tutorial.optionalParameters.destroyOnEnd = EditorGUILayout.Toggle(new GUIContent("Destroy on end", "Destroys this tutorial system when finished. Please note that this will break all tutorial component references in scene. For example custom restart button"), tutorial.optionalParameters.destroyOnEnd);
            tutorial.optionalParameters.overlayColor = EditorGUILayout.ColorField(new GUIContent("Overlay color", "Default user interface overlay color"), tutorial.optionalParameters.overlayColor);
            tutorial.optionalParameters.saveMethod = (TutorialSystemSaveMethod)EditorGUILayout.EnumPopup(new GUIContent("Save Method", "None = save disabled\r\nPlayerPref = saves progress to player preferences"), tutorial.optionalParameters.saveMethod);          
            EditorGUILayout.LabelField("Text typing effect. Set to 0 for no effect");
            tutorial.optionalParameters.textDisplaySpeed = EditorGUILayout.Slider(tutorial.optionalParameters.textDisplaySpeed, 0.00f, 0.15f);
            tutorial.optionalParameters.displayImage = EditorGUILayout.Toggle(new GUIContent("Use image", "Enable to display image inside dialog window"), tutorial.optionalParameters.displayImage);
            tutorial.optionalParameters.defaultImage = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Default image", "Default image that is used in dialog window"), tutorial.optionalParameters.defaultImage, typeof(Sprite), true);
        }

        ShowTutorials(serializedObject.FindProperty("tutorialList"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);
        }

        serializedObject.Update();
    }

    public void ShowTutorials(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);

        //if (list.arraySize > 0)
        //{
        //    EditorGUILayout.LabelField("Tutorials List");
        //    EditorGUI.indentLevel += 1;
        //    for (int i = 0; i < list.arraySize; i++)
        //    {
        //        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent("Tutorial: " + (i).ToString()));
        //    }
        //    EditorGUI.indentLevel -= 1;
        //}
    }
}
