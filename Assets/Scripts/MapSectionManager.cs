using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MapSectionManager : MonoBehaviour {
    public float velocity = 15f;
    public GameObject mapSection;
    public int sectionsAhead = 5;
    private List<GameObject> activeSections = new List<GameObject>();
    public float destroyDistance = 50f;
    private int currentSectionID = 0;
    ObstacleSpawner obsSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        obsSpawner = GetComponent<ObstacleSpawner>();
        obsSpawner.CalculateBorders(mapSection);

        if (sectionsAhead < 2) {
            Debug.LogError("sectionsAhead must be at least 2");
            sectionsAhead = 2;
        }
        Debug.Log("Map size: " + mapSection.GetComponentsInChildren<Renderer>()[0].bounds.size);

        GenerateSectionsOnStart();
    }

    void Update() {
        for (int i = 0; i < sectionsAhead; i++) {
            GameObject section = activeSections[i];
            Rigidbody sectionRB = section.GetComponent<Rigidbody>();
            Collider renderer = section.GetComponentsInChildren<Collider>()[0];

            if (renderer.bounds.max.x >= destroyDistance) {
                // destroy the section and generate a new one
                GenerateNewMapSection();
                // obsSpawner.RemoveSectionReferences(section);
                Destroy(section);
                activeSections.Remove(section);
            }
        }

        obsSpawner.Refresh();
    }

    void FixedUpdate() {
        foreach (GameObject section in activeSections) {
            Rigidbody rb = section.GetComponent<Rigidbody>();
            rb.MovePosition(rb.position + new Vector3(velocity, 0, 0) * Time.deltaTime);
        }
    }

    private void GenerateNewMapSection() {
        int numActiveSections = activeSections.Count;
        GameObject newSection;

        if (numActiveSections == 0) {
            // generate the first section at the origin
            newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity, transform);
            newSection.transform.position = transform.position;

            newSection.name = "MapSection_" + currentSectionID;
            MapSectionID IDComponent = newSection.GetComponent<MapSectionID>();
            IDComponent.sectionID = currentSectionID;
            currentSectionID++;
        }
        else {
            //get the last section to determine the position of the new section
            GameObject lastSection = activeSections[numActiveSections - 1];

            // a renderer is needed to get the bounds of a section
            Collider lastSectionCollider = lastSection.GetComponentsInChildren<Collider>()[0];

            // instantiate a new section at 0, 0, 0 as a child of the map section manager
            newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity, transform);

            Vector3 newPosition;
            float newX;

            newX = lastSection.transform.position.x - lastSectionCollider.bounds.size.x;
            newPosition = new Vector3(newX, lastSection.transform.position.y, lastSection.transform.position.z);
            newSection.transform.position = newPosition;

            newSection.name = "MapSection_" + currentSectionID;
            MapSectionID IDComponent = newSection.GetComponent<MapSectionID>();
            IDComponent.sectionID = currentSectionID;
            currentSectionID++;

            obsSpawner.PopulateSection(newSection);
        }

        activeSections.Add(newSection);
    }

    private void GenerateSectionsOnStart() {
        int numActiveSections = GetActiveSectionsOnStart().Count;

        if (mapSection == null) {
            Debug.LogWarning("mapSection is not assigned.");
            return;
        }

        int sectionsToGenerate = sectionsAhead - numActiveSections;
        currentSectionID = numActiveSections;

        // generate the sections ahead
        for (int i = 0; i < sectionsToGenerate; i++) {
            GenerateNewMapSection();
        }
    }

    private List<GameObject> GetActiveSectionsOnStart() {
        activeSections.Clear();
        foreach (Transform child in transform)
            activeSections.Add(child.gameObject);

        return activeSections;
    }

    public GameObject GetSectionByID(int ID) {
        foreach (GameObject ms in activeSections) {
            if (ms.GetComponent<MapSectionID>().sectionID == ID)
                return ms;
        }
        return null;
    }

    public void ResetCount() {
        currentSectionID = 0;
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