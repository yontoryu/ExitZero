using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MapSectionManager : MonoBehaviour {
    public GameObject mapSection;
    public int sectionsAhead = 5;
    private List<GameObject> activeSections = new List<GameObject>();
    public float destroyDistance = 50f;
    private int currentSectionID = 1;
    public ObstacleSpawner obstacleSpawner;

    [Header("Difficulty Settings")]
    public float velocity = 15f;
    private float startVelocity;
    private float endVelocity;
    public float velocityFactor = 0.02f;
    private float difficulty;
    public int maxVelocityTime = 220;

    void Awake() {
        difficulty = PlayerPrefs.GetFloat("Pace", 0.5f);
        Debug.Log("diff " + difficulty);

        startVelocity = GetStartVelocity(difficulty);
        endVelocity = GetEndVelocity(difficulty);
    }

    private float GetStartVelocity(float difficulty) {
        return 5 * difficulty + 15;
    }

    private float GetEndVelocity(float difficulty) {
        return 12 * difficulty + 18;
    }

    private float GetCurrentVelocity() {
        float t = Time.time;
        if (t > maxVelocityTime) {
            return endVelocity;
        }
        else {
            return (endVelocity - startVelocity) / maxVelocityTime * t + startVelocity;
        }
    }

    public void SetVelocity() {
        velocity = GetCurrentVelocity();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        obstacleSpawner.CalculateBorders(mapSection);

        if (sectionsAhead < 2) {
            Debug.LogError("sectionsAhead must be at least 2");
            sectionsAhead = 2;
        }
        Debug.Log("Map size: " + mapSection.GetComponentsInChildren<Renderer>()[0].bounds.size);

        GenerateSectionsOnStart();
    }

    void Update() {
        SetVelocity();
    }

    void FixedUpdate() {
        for (int i = 0; i < sectionsAhead; i++) {
            obstacleSpawner.Refresh();
            GameObject section = activeSections[i];
            Rigidbody sectionRB = section.GetComponent<Rigidbody>();
            Collider renderer = section.GetComponentsInChildren<Collider>()[0];

            if (renderer.bounds.max.x >= destroyDistance) {
                // destroy the section and generate a new one
                Destroy(section);
                GenerateNewMapSection();
                activeSections.Remove(section);
            }
            sectionRB.MovePosition(sectionRB.position + new Vector3(velocity, 0, 0) * velocityFactor);
        }
    }

    private void GenerateNewMapSection() {
        int numActiveSections = activeSections.Count;
        GameObject newSection;

        if (numActiveSections == 0) {
            // generate the first section at the origin
            newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity, transform);
            newSection.GetComponent<Rigidbody>().position = transform.position;

            newSection.name = "MapSection_" + currentSectionID;
            MapSectionID IDComponent = newSection.GetComponent<MapSectionID>();
            IDComponent.sectionID = currentSectionID;
            currentSectionID++;
        }
        else {
            // instantiate a new section at 0, 0, 0 as a child of the map section manager
            newSection = Instantiate(mapSection, Vector3.zero, Quaternion.identity, transform);

            newSection.name = "MapSection_" + currentSectionID;
            MapSectionID IDComponent = newSection.GetComponent<MapSectionID>();
            IDComponent.sectionID = currentSectionID;
            currentSectionID++;

            //get the last section to determine the position of the new section
            GameObject lastSection = activeSections[numActiveSections - 1];
            Rigidbody lastRB = lastSection.GetComponent<Rigidbody>();
            Collider lastSectionCollider = lastSection.GetComponentsInChildren<Collider>()[0];
            float sectionLength = lastSectionCollider.bounds.size.x;

            float newX = lastRB.position.x - sectionLength - 0.001f; // minimale Einheit z. B. 1mm Abstand
            Vector3 newPosition = new Vector3(newX, lastRB.position.y, lastRB.position.z);
            newSection.GetComponent<Rigidbody>().position = newPosition;

            StartCoroutine(SpawnObstaclesDelayed(newSection));
        }

        activeSections.Add(newSection);
    }

    private IEnumerator SpawnObstaclesDelayed(GameObject section) {
        yield return new WaitForFixedUpdate(); // wartet den nächsten Physik-Zyklus ab
        obstacleSpawner.PopulateSection(section);
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

    public int GetID(GameObject mapSection) {
        return mapSection.GetComponent<MapSectionID>().sectionID;
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