using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UIScreenDataBase", menuName = "Scriptable Objects/UI/UIScreenDataBase")]
public class ScreenDataBase : ScriptableObject
{
    public const string SCREEN_PREFAB_PATH = "Assets/Prefabs/UI/Screens";

    [ReadOnly]
    public List<GameObject> RegisteredScreens;

    public Dictionary<Type, GameObject> UIScreens = new Dictionary<Type, GameObject>();

#if UNITY_EDITOR
    public void UpdateScreenList()
    {
        RegisteredScreens.Clear();
        RegisteredScreens = new List<GameObject>();

        var prefabGuids = UnityEditor.AssetDatabase.FindAssets("t:prefab", new[] { SCREEN_PREFAB_PATH });

        foreach (var prefabGuid in prefabGuids)
        {
            string prefabPath = UnityEditor.AssetDatabase.GUIDToAssetPath(prefabGuid);
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var screen = prefab.GetComponent<IScreen>();

            if (screen != null)
            {
                RegisteredScreens.Add(prefab);
            }
        }
    }
#endif
    public void SetUp()
    {
        foreach (GameObject screenPrefab in RegisteredScreens)
        {
            IScreen screen = screenPrefab.GetComponent<IScreen>();
            Type type = screen.GetType();
            UIScreens.Add(type, screenPrefab);
        }
    }
}