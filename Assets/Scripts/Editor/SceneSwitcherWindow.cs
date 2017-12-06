using UnityEngine;
using UnityEditor;
using System.Collections;

public class SceneSwitcherWindow : EditorWindow
{
    #region PRIVATE_VARIABLES
    private Vector2 scrollPosition;
    #endregion

    #region UNITY_EVENTS
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
        DrawScenes();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    #endregion

    void DrawScenes()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                if (GUILayout.Button(scene.path) && EditorApplication.SaveCurrentSceneIfUserWantsTo())
                {
                    EditorApplication.OpenScene(scene.path);
                }
            }
        }
    }

}
