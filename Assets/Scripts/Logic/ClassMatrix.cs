using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClassMatrix : ScriptableObject
{
    [HideInInspector]
    public string[] names = Enum.GetNames(typeof(Classification.Type));
    [SerializeField, HideInInspector]
    private bool[] matrix = new bool[7*7];
    public List<string> info;

    public int GetEnumPos(Classification.Type type)
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i] == type.ToString())
            {
                return i;
            }
        }
        return 999;
    }

    public bool GetIgnoreLayerCollision(int x, int y)
    {
        if (x <= names.Length && y <= names.Length) {
            return matrix[names.Length * y + + x];
        }
        return false;
    }
    public void SetIgnoreLayerCollision(int x, int y, bool val)
    {
        if (x <= names.Length && y <= names.Length)
        {
            matrix[names.Length * y + x] = val;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
    public List<Classification.Type> StrongAgainst(Classification.Type type)
    {
        int row = GetEnumPos(type);
        List<string> n = new List<string>();

        for (int i = 0; i < 7; i++)
        {
            if (matrix[names.Length * i + row] == false)
            {
                n.Add(names[i]);
            }
        }

        List<Classification.Type> arr = new List<Classification.Type>();
        foreach (var i in n)
        {
            arr.Add((Classification.Type)Enum.Parse(typeof(Classification.Type), i));
        }

        return arr;
    }

    public List<Classification.Type> WeakTo(Classification.Type type)
    {
        int col = GetEnumPos(type);        
        List<string> n = new List<string>();

        for (int i = 0; i < 7; i++)
        {
            if (matrix[names.Length * col + i] == false)
            {
                n.Add(names[i]);
            }
        }

        List<Classification.Type> arr = new List<Classification.Type>();
        foreach (var i in n)
        {
            arr.Add((Classification.Type)Enum.Parse(typeof(Classification.Type), i));
        }

        return arr;
    }
}