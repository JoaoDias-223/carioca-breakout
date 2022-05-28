using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MeshFromPolygonCollider2D))]
public class MeshFromPolygonCollider2DEditor : Editor
{
    public  void OnSceneGUI()
    {
        // enable update when editing mesh collider in Editor
        MeshFromPolygonCollider2D myTarget = (MeshFromPolygonCollider2D)target;
        myTarget.Init();
    }
}
