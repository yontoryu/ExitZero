#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapSectionManager))]
public class MapSectionManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MapSectionManager manager = (MapSectionManager)target;

        if (GUILayout.Button("Generate Sections")) {
            manager.ClearSections(); // Clear existing sections before generating new ones
            manager.GenerateSectionsAhead();
        }

        if (GUILayout.Button("Clear Sections")) {
            manager.ClearSections();
        }
    }
}
#endif
