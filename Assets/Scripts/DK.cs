using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DK : MonoBehaviour
{
    public GameObject prefab;
    private float minTime = 2f;
    private float maxTime = 4f;
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Spawn(){
        Vector2 position = transform.position;
        position.x += 1.5f;
        position.y -= 0.65f;
        Instantiate(prefab, position, Quaternion.identity);
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));
    }
}
