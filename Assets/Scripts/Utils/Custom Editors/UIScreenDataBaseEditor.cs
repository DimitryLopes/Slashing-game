#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScreenDataBase))]
public class ScreenDataBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ScreenDataBase dataBase = (ScreenDataBase)target;

        if (GUILayout.Button("Update Screen List"))
        {
            dataBase.UpdateScreenList();
            EditorUtility.SetDirty(dataBase);
        }
    }
}
#endif