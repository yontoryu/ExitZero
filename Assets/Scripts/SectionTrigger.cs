using JetBrains.Annotations;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject mapSection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("mapSectionTrigger")) {
            Instantiate(mapSection, new Vector3(-26, 0, 0), Quaternion.identity);
        }
    }
}
