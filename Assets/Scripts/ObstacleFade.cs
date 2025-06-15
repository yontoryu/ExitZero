using UnityEngine;

public class ObstacleFade : MonoBehaviour {
    public ObstacleFadeSO obstacleFade;
    private float startFadeX, endFadeX, alphaMin;
    private Renderer rend;
    private Material material;

    void Start() {
        rend = GetComponent<Renderer>();
        material = rend.material; // Achtung: erzeugt Instanz → nicht sharedMaterial verwenden
        SetAlpha(1f); // Volle Sichtbarkeit zu Beginn
        startFadeX = obstacleFade.startFadeX;
        endFadeX = obstacleFade.endFadeX;
        alphaMin = obstacleFade.alphaMin;
    }

    void Update() {
        float x = transform.position.x;
        float alpha = 1f;

        if (x > startFadeX) {
            float t = Mathf.InverseLerp(startFadeX, endFadeX, x);
            alpha = Mathf.Lerp(1f, alphaMin, t);
        }

        SetAlpha(alpha);
        Debug.Log("obs: " + name + " alpha: " + alpha);
    }

    void SetAlpha(float a) {
        if (material.HasProperty("_BaseColor")) {
            Color color = material.GetColor("_BaseColor");
            color.a = a;
            material.SetColor("_BaseColor", color);
        }
    }

}
