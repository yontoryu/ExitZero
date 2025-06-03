using System.Collections.Generic;
using UnityEngine;

public class MapSectionManager : MonoBehaviour {
    public float velocity = 15f;
    public GameObject mapSection;
    public int sectionsAhead = 5;
    private List<GameObject> activeSections = new List<GameObject>();
    public float destroyDistance = 50f;

    public void ClearSections() {
        GetActiveSections();

        for (int i = activeSections.Count - 1; i >= 0; i--) {
            if (activeSections[i] != null) {
#if UNITY_EDITOR
                DestroyImmediate(activeSections[i]);
#else
                Destroy(activeSections[i]); // in a build, use Destroy instead of DestroyImmediate
#endif
            }
        }
        activeSections.Clear();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (sectionsAhead < 2) {
            Debug.LogError("sectionsAhead must be at least 2");
            sectionsAhead = 2;
        }

        GenerateSectionsAhead();
    }

    // Update is called once per frame
    void Update() {
        for (int i = activeSections.Count - 1; i >= 0; i--) {
            GameObject section = activeSections[i];

            // move the section
            section.transform.position += new Vector3(velocity, 0, 0) * Time.deltaTime;

            // check if the section is out of bounds
            Renderer sectionRenderer = section.GetComponentsInChildren<Renderer>()[0];
            if (sectionRenderer.bounds.max.x >= destroyDistance) {
                // destroy the section and generate a new one
                GameObject newSection = GenerateNewMapSection();
                activeSections.Add(newSection);
                Destroy(section);
                activeSections.Remove(section);
            }
        }
    }

    private GameObject GenerateNewMapSection() {
        int numActiveSections = activeSections.Count;
        GameObject newSection;

        // if there are already sections
        if (numActiveSections > 0) {
            //get the last section to determine the position of the new section
            GameObject lastSection = activeSections[numActiveSections - 1];

            // a renderer is needed to get the bounds of a section
            Renderer lastSectionRenderer = lastSection.GetComponentsInChildren<Renderer>()[0];

            // get the x position of the end of the section
            float XEndOfMapSection = lastSectionRenderer.bounds.min.x;

            // instantiate a new section at 0, 0, 0 as a child of the map section manager
            newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity, transform);
            newSection.name = "MapSection_" + activeSections.Count;

            // get the renderer of the new section to set its position correctly
            Renderer newSectionRenderer = newSection.GetComponentsInChildren<Renderer>()[0];

            // the x position of the new section should be equal to the x position of the end of the last one subtracted by half of the width of a section
            float newX = XEndOfMapSection - newSectionRenderer.bounds.extents.x;
            Vector3 newPosition = new Vector3(newX, lastSection.transform.position.y, lastSection.transform.position.z);

            newSection.transform.position = newPosition;
        }
        else {
            // generate the first section at the origin
            newSection = Instantiate(mapSection, transform.position, Quaternion.identity, transform);
            newSection.name = "MapSection_0";
        }

        return newSection;
    }

    public void GenerateSectionsAhead() {
        int numActiveSections = GetActiveSections();

        if (mapSection == null) {
            Debug.LogWarning("mapSection is not assigned.");
            return;
        }

        int sectionsToGenerate = sectionsAhead - numActiveSections;

        // generate the sections ahead
        for (int i = 0; i < sectionsToGenerate; i++) {
            GameObject newSection = GenerateNewMapSection();
            activeSections.Add(newSection);
        }
    }

    public int GetActiveSections() {
        activeSections.Clear();
        foreach (Transform child in transform)
            activeSections.Add(child.gameObject);

        return activeSections.Count;
    }

    void OnDrawGizmos() {
        // Draw a line to visualize the destroy distance
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(destroyDistance, -5, -8), new Vector3(destroyDistance, 5, -8));
        Gizmos.DrawLine(new Vector3(destroyDistance, -5, 8), new Vector3(destroyDistance, 5, 8));
        Gizmos.DrawLine(new Vector3(destroyDistance, 5, -8), new Vector3(destroyDistance, 5, 8));
        Gizmos.DrawLine(new Vector3(destroyDistance, -5, -8), new Vector3(destroyDistance, -5, 8));
    }
}