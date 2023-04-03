using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float minTime = 2f;
    public float maxTime = 4f;

    private void Start()
    {
        Spawn();    
    }

    private void Spawn() {
        // Spawn barrels
        Instantiate(prefab, transform.position, Quaternion.identity);

        // Call this function again
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));
    }
}
