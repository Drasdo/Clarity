/*
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BranchingStoryController))]
[CanEditMultipleObjects]
public class BranchingStoryControllerEditor : Editor {

    SerializedProperty numOfLevels;

    SerializedProperty videoStructure;

    SerializedProperty TextSelector;
    SerializedProperty videoSphere;

    private int arrayCounter;

    void OnEnable()
    {
        numOfLevels = serializedObject.FindProperty("numOfLevels");

        videoStructure = serializedObject.FindProperty("videoStructure");

        TextSelector = serializedObject.FindProperty("TextSelector");
        videoSphere = serializedObject.FindProperty("videoSphere");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(TextSelector);
        EditorGUILayout.PropertyField(videoSphere);
        EditorGUILayout.LabelField(" ");
        EditorGUILayout.PropertyField(numOfLevels);

        EditorGUILayout.PropertyField(videoStructure);

        
        if(numOfLevels != null)//once this has something in it;
        {
            //we need to set the size of the arrays for each of the fields based on the number above
            arrayCounter = 0;
            for (int i = 0; i <= numOfLevels.intValue; i++) //look through number of trees, need to get to level 3.
            {
                EditorGUILayout.LabelField("Level " + i);
                EditorGUI.indentLevel++;
                for (int j = 0; j <= Mathf.Pow(2,i-2); j++) //number of choices increases per level at 2^i-1 (i being current level)
                {
                    EditorGUILayout.LabelField("Video: " + i + "-" + j + " (" + arrayCounter + ")"); //need to make this clearer somehow to people know which video is what.
                    //Show field for this video option
                    //this looks complicated, but done this way just for formatting reasons. plz ignore

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    GUILayout.Label("Sphere Video Path Left: ", GUILayout.Width(150));
                    GUILayout.FlexibleSpace();
                    

                    arrayCounter++;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField(" ");
            }
        }
        serializedObject.ApplyModifiedProperties();
        
    }

}
*/