using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ClassMatrix))]
public class ClassMatrixInspector : Editor {
    private Vector2 scrollPos;

    private bool show = true;
    private bool GetValue(int layerA, int layerB)
    {
        ClassMatrix script = (ClassMatrix)target;
        return !script.GetIgnoreLayerCollision(layerA, layerB);
    }

    private void SetValue(int layerA, int layerB, bool val)
    {
        ClassMatrix script = (ClassMatrix)target;
        script.SetIgnoreLayerCollision(layerA, layerB, !val);
    }

    public override void OnInspectorGUI()
    {
        ClassMatrix script = (ClassMatrix)target;
        base.DrawDefaultInspector();
        script.names = script.names.Take(7).ToArray();
        MatrixGUI.DoGUI("Matrix", ref this.show, ref this.scrollPos, new MatrixGUI.GetValueFunc(this.GetValue), new MatrixGUI.SetValueFunc(this.SetValue), script.names);
    }
}