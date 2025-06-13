#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(MapSectionManager))]
public class MapSectionManagerEditor : Editor {
    // private List<GameObject> activeSections = new List<GameObject>();
    // public override void OnInspectorGUI() {
    //     DrawDefaultInspector();

    //     MapSectionManager msManager = (MapSectionManager)target;

    //     if (GUILayout.Button("Generate Sections")) {
    //         ClearSections(); // Clear existing sections before generating new ones
    //         msManager.GenerateSectionsOnStart();
    //     }

    //     if (GUILayout.Button("Clear Sections")) {
    //         ClearSections();
    //     }
    // }

    // void ClearSections() {
    //     GetActiveSections();
    //     MapSectionManager msManager = (MapSectionManager)target;
    //     msManager.ResetCount();

    //     for (int i = activeSections.Count - 1; i >= 0; i--) {
    //         if (activeSections[i] != null) {
    //             DestroyImmediate(activeSections[i]);
    //         }
    //     }
    //     activeSections.Clear();
    // }

    // private int GetActiveSections() {
    //     MapSectionManager msManager = (MapSectionManager)target;

    //     activeSections.Clear();
    //     foreach (Transform child in msManager.transform)
    //         activeSections.Add(child.gameObject);

    //     return activeSections.Count;
    // }
}
#endif
