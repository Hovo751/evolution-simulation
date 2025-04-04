using UnityEngine;

public class Bush : MonoBehaviour
{
    public float berries = 3;
    public float hunger = 0.5f;
    public float growSpeed = 1;
    public GameObject berry1;
    public GameObject berry2;
    public GameObject berry3;
    void Update()
    {
        if (berries >= 1) berry1.SetActive(true); else berry1.SetActive(false);
        if (berries >= 2) berry2.SetActive(true); else berry2.SetActive(false);
        if (berries >= 3) berry3.SetActive(true); else berry3.SetActive(false);
        berries += Time.deltaTime * growSpeed;
        berries = Mathf.Min(berries, 3);
    }
}
