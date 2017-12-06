using UnityEngine;
using UnityEditor;
using System.IO;

public class ProfileEditor : Editor
{
    private ProfileEditor profileEditor;

    [MenuItem("Tools/Profile Editor/Delete Current %#d")]
    static void DeleteCurrent()
    {
        File.Delete(Application.persistentDataPath + "/save.xml");
    }

    [MenuItem("Tools/Profile Editor/Open saves folder")]
    static void OpenFolder()
    {
        System.Diagnostics.Process.Start("explorer.exe", "/select," + Application.persistentDataPath);
    }
}
