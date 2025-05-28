using System.Collections.Generic;
using UnityEngine;

public class MapSectionManager : MonoBehaviour {
    public float velocity = 15f;
    public GameObject firstSection;
    public GameObject mapSection;
    public int sectionsAhead = 5;
    private List<GameObject> activeSections = new List<GameObject>();
    public float destroyDistance = 50f;

    public void ClearSections() {
        for (int i = activeSections.Count - 1; i >= 0; i--) {
            if (activeSections[i] != null && activeSections[i] != firstSection) {
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
            if (sectionRenderer.bounds.center.x >= destroyDistance) {
                // destroy the section and generate a new one
                GameObject newSection = GenerateNewMapSection(activeSections[sectionsAhead - 1]);
                activeSections.Add(newSection);
                Destroy(section);
                activeSections.Remove(section);
            }
        }
    }

    private GameObject GenerateNewMapSection(GameObject lastSection) {
        // a renderer is needed to get the bounds of a section
        Renderer lastSectionRenderer = lastSection.GetComponentsInChildren<Renderer>()[0];

        // get the x position of the end of the section
        float XEndOfMapSection = lastSectionRenderer.bounds.min.x;

        // instantiate a new section at 0, 0, 0
        GameObject newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity);
        newSection.name = "MapSection_" + activeSections.Count;

        // get the renderer of the new section to set its position correctly
        Renderer newSectionRenderer = newSection.GetComponentsInChildren<Renderer>()[0];

        // the x position of the new section should be equal to the x position of the end of the last one subtracted by half of the width of a section
        float newX = XEndOfMapSection - newSectionRenderer.bounds.extents.x;
        Vector3 newPosition = new Vector3(newX, lastSection.transform.position.y, lastSection.transform.position.z);

        newSection.transform.position = newPosition;

        return newSection;
    }

    public void GenerateSectionsAhead() {
        if (firstSection == null || mapSection == null) {
            Debug.LogWarning("firstSection or mapSection is not assigned.");
            return;
        }

        // generate the first section
        activeSections.Add(firstSection);

        // generate the rest of the sections ahead
        for (int i = 0; i < sectionsAhead - 1; i++) {
            GameObject newSection = GenerateNewMapSection(activeSections[i]);
            activeSections.Add(newSection);
        }
    }
}