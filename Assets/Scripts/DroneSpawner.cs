using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public GameObject[] dronePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Instantiate(dronePrefab[Random.Range(0, dronePrefab.Length)], this.transform.position + new Vector3(0, 30, 0), Quaternion.identity);
        }
    }
}
