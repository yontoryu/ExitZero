using System.Collections.Generic;
using UnityEngine;

public class MapSectionManager : MonoBehaviour {
    public float velocity = 15;
    public GameObject firstSection;
    public GameObject mapSection;
    public int sectionsAhead = 5;
    List<GameObject> activeSections = new List<GameObject>();
    public float destroyDistance = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        // generate the first section
        activeSections.Add(firstSection);
        for (int i = 0; i < sectionsAhead - 1; i++) {
            GameObject newSection = GenerateNewMapSection(activeSections[i]);
            activeSections.Add(newSection);
        }
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
        GameObject newSection;

        Renderer lastSectionRenderer = lastSection.GetComponentsInChildren<Renderer>()[0];

        // get the x position of the end of the section
        float XEndOfMapSection = lastSectionRenderer.bounds.min.x;

        // instantiate a new section at 0, 0, 0
        newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity);

        Renderer newSectionRenderer = newSection.GetComponentsInChildren<Renderer>()[0];

        // the x position of the new section should be equal to the x position of the end of the last one subtracted by half of the width of a section
        float newX = XEndOfMapSection - newSectionRenderer.bounds.extents.x;
        Vector3 newPosition = new Vector3(newX, lastSection.transform.position.y, lastSection.transform.position.z);

        newSection.transform.position = newPosition;
        Debug.Log("New section position: " + newSection.transform.position);

        return newSection;
    }
}