using UnityEngine;
using System.Collections;
using UnityEditor;

public class Tools : Editor {
	public static SceneSwitcherWindow sceneSwicherWindow;
	[MenuItem("Tools/Scene Switcher")]
	static void Init()
	{
		sceneSwicherWindow = (SceneSwitcherWindow)EditorWindow.GetWindow(typeof(SceneSwitcherWindow));
		sceneSwicherWindow.titleContent = new GUIContent("Scene Switcher");
	}
}
