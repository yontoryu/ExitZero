 using UnityEngine;

public class MoveSection : MonoBehaviour
{
    public float velocity = 15;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(velocity, 0, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy")) {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player")) {
            Instantiate(gameObject, new Vector3(-28.45037f, 0, 0), Quaternion.identity);
            Debug.Log("test");
        }
    }
}
