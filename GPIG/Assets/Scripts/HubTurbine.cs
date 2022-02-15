using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubTurbine : WindTurbine
{
    public Drone[] charging = new Drone[9];
    public float chargeRate = 0.00001F;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

        // Charge drones
        foreach (Drone d in charging){
            if (d!=null){
                d.battery_percentage+= chargeRate;
            }
        }
    }
}
