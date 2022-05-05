using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneIconFollow : MonoBehaviour
{
    private Quaternion iconRotation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        iconRotation = Quaternion.Euler(90, 0, 0);

        this.transform.rotation = iconRotation;
    }
}
