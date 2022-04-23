using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandingPad
{
    public float chargeRate = 0.00001F;
    public List<GameObject> charging_pads = new List<GameObject>();
    public int num_chargepads;
    // GameObject[] non_charging_pads;


    public LandingPad(int num_chargepads, int num_nonchargepads){
        this.num_chargepads = num_chargepads;
        // non_charging_pads= new GameObject[num_nonchargepads];
    }


    public void Update(){
        // Charge Drones
        foreach (GameObject drone in charging_pads){
            if (drone != null){
                //get the script
                Drone droneScript = drone.GetComponent<Drone>();
                if (droneScript!=null && droneScript.battery_percentage < 100F){
                    droneScript.battery_percentage+= chargeRate;
                    if (droneScript.battery_percentage > 100F){
                        droneScript.battery_percentage= 100F;
                    }
                }
            }
        }

    }

    public bool holdChargingDrone(GameObject drone, int ind){
        if (charging_pads.Count < num_chargepads) {
            charging_pads.Add(drone);
            return true;
        }
        return false;
    }

    // public bool holdNonChargingObject(GameObject obj, int ind){
    //     if (non_charging_pads[ind]!=null){
    //         non_charging_pads[ind]= obj;
    //         return true;
    //     }
    //     return false;
    // }


    public GameObject releaseChargingDrone(int ind){
        GameObject res = charging_pads[ind];
        charging_pads[ind]= null;
        return res;
    }

    // public GameObject releaseNonChargingObject(int ind){
    //     GameObject res = non_charging_pads[ind];
    //     non_charging_pads[ind]= null;
    //     return res;
    // }


    public int openChargingPad(){
        if (charging_pads.Count + 1 < num_chargepads) {
            return charging_pads.Count + 1;
        }
        return -1;
    }

    // public int openNonChargingPad(){
    //     for (int i=0; i<non_charging_pads.Length; i++){
    //         if (non_charging_pads[i]==null){
    //             return i;
    //         }
    //     }
    //     return -1;
    // }

}
