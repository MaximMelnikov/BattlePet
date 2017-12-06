using UnityEngine;
using UnityEditor;

public class ScriptableCreator
{
    [MenuItem("Assets/Create/Creature")]
    public static void CreateCreature()
    {
        CreatureSettings asset = ScriptableObject.CreateInstance<CreatureSettings>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Settings/Creatures/NewCreature.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/Ability")]
    public static void CreateSpell()
    {
        AbilitySettings asset = ScriptableObject.CreateInstance<AbilitySettings>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Settings/Abilities/NewAbility.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }

    [MenuItem("Assets/Create/ClassMatrix")]
    public static void CreateClassMatrix()
    {
        ClassMatrix asset = ScriptableObject.CreateInstance<ClassMatrix>();
        AssetDatabase.CreateAsset(asset, "Assets/ClassMatrix.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}