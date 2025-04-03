using UnityEngine;

public class MoveSection : MonoBehaviour {
    public float velocity = 15;
    public GameObject mapSection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.position += new Vector3(velocity, 0, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Destroy")) {
            Destroy(gameObject);
            Debug.Log("destroyed map section");
        }
        if (other.gameObject.CompareTag("Player")) {
            GenerateNewMapSection();
        }
    }

    private void GenerateNewMapSection() {
        Renderer renderer = gameObject.GetComponentsInChildren<Renderer>()[0];
        if (renderer == null) return;

        float XEndOfMapSection = renderer.bounds.min.x;

        GameObject newSection = Instantiate(gameObject, Vector3.zero, Quaternion.identity);

        Renderer newRenderer = newSection.GetComponentsInChildren<Renderer>()[0];
        if (newRenderer == null) return;

        float newX = XEndOfMapSection - newRenderer.bounds.extents.x;
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);

        newSection.transform.position = newPosition;

        Debug.Log("new map section generated at position " + newPosition);
    }
}
