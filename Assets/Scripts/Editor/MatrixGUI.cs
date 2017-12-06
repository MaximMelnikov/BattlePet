using System;
using UnityEditor;
using UnityEngine;
internal class MatrixGUI
{
    public delegate bool GetValueFunc(int layerA, int layerB);

    public delegate void SetValueFunc(int layerA, int layerB, bool val);

    public static void DoGUI(string title, ref bool show, ref Vector2 scrollPos, MatrixGUI.GetValueFunc getValue, MatrixGUI.SetValueFunc setValue, string[] names)
    {
        int num = 100;
        int num2 = 0;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] != "")
            {
                num2++;
            }
        }
        for (int j = 0; j < names.Length; j++)
        {
            Vector2 vector = GUI.skin.label.CalcSize(new GUIContent(names[j]));
            if ((float)num < vector.x)
            {
                num = (int)vector.x;
            }
        }
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.Space(0f);
        show = EditorGUILayout.Foldout(show, title, true);
        GUILayout.EndHorizontal();
        if (show)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, new GUILayoutOption[]
            {
                    GUILayout.MinHeight((float)(num + 20)),
                    GUILayout.MaxHeight((float)(num + (num2 + 1) * 16))
            });
            Rect rect = GUILayoutUtility.GetRect((float)(16 * num2 + num), (float)num);

            //GUILayout.BeginArea(rect);
            Rect topmostRect = rect;
            Vector2 vector2 = new Vector2(rect.xMin+15, rect.yMax+5);
            int num3 = 0;
            for (int k = 0; k < names.Length; k++)
            {
                if (names[k] != "")
                {
                    float num4 = (float)(num + 30 + (num2 - num3) * 16) - (topmostRect.width + scrollPos.x);
                    if (num4 < 0f)
                    {
                        num4 = 0f;
                    }
                    Vector3 pos = new Vector3((float)(num + 30 + 16 * (num2 - num3)) + vector2.y + vector2.x + scrollPos.y - num4, vector2.y + scrollPos.y, 0f);
                    GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one) * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                    if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9.0"))
                    {
                        GUI.matrix *= Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity, Vector3.one);
                    }
                    //GUI.Label(new Rect(2f - vector2.x - scrollPos.y, scrollPos.y - num4, (float)num, 16f), names[k], "RightLabel");
                    num3++;
                }
            }
            GUI.matrix = Matrix4x4.identity;
            num3 = 0;
            for (int l = 0; l < names.Length; l++)
            {
                if (names[l] != "")
                {
                    int num5 = 0;
                    Rect rect2 = GUILayoutUtility.GetRect((float)(30 + 16 * num2 + num), 16f);
                    GUI.Label(new Rect(rect2.x + 30f, rect2.y, (float)num, 16f), names[l], "RightLabel");
                    for (int m = names.Length-1; m >= 0; m--)
                    {
                        if (names[m] != "")
                        {
                            //if (num5 < num2 - num3)
                            //{
                                GUIContent content = new GUIContent("", names[l] + "/" + names[m]);
                                bool flag = getValue(l, m);
                                bool flag2 = GUI.Toggle(new Rect((float)(num + 30) + rect2.x + (float)(num5 * 16), rect2.y, 16f, 16f), flag, content);
                                if (flag2 != flag)
                                {
                                    setValue(l, m, flag2);
                                }
                            //}
                            num5++;
                        }
                    }
                    num3++;
                }
            }
            //GUILayout.EndArea();
            GUILayout.EndScrollView();
        }
    }
}