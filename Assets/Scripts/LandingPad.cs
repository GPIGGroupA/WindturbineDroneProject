using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandingPad
{
    public float chargeRate = 0.00001F;
    public int num_chargepads;
    public List<GameObject> charging_pads = new List<GameObject>();


    public LandingPad(int p_num_chargepads){
        num_chargepads = p_num_chargepads;
    }

    public void Update(){
        // Charge Drones
        foreach (GameObject drone in charging_pads){
            Drone droneScript = drone.GetComponent<Drone>();
            if (droneScript!=null && droneScript.battery_percentage < 100F){
                droneScript.battery_percentage+= chargeRate;
                if (droneScript.battery_percentage > 100F){
                    droneScript.battery_percentage= 100F;
                }
            }
        }

    }

    public bool holdChargingDrone(GameObject drone){
        if (charging_pads.Count < num_chargepads) {
            charging_pads.Add(drone);
            return true;
        }
        return false;
    }

    public GameObject? releaseChargingDrone(int ind){
        if (ind >= charging_pads.Count) {
            return null;
        } else {
            GameObject res = charging_pads[ind];
            charging_pads.RemoveAt(ind);
            return res;
        }
    }

}
