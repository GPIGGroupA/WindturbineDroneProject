using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandingPad
{
    public float chargeRate = 0.00001F;
    Drone[] charging_pads;
    // GameObject[] non_charging_pads;


    public LandingPad(int num_chargepads, int num_nonchargepads){
        charging_pads= new Drone[num_chargepads];
        // non_charging_pads= new GameObject[num_nonchargepads];
    }


    public void Update(){

        // Charge Drones
        foreach (Drone drone in charging_pads){
            if (drone!=null && drone.battery_percentage < 100F){
                drone.battery_percentage+= chargeRate;
                if (drone.battery_percentage > 100F){
                    drone.battery_percentage= 100F;
                }
            }
        }

    }


    public bool holdChargingDrone(Drone drone, int ind){
        if (charging_pads[ind]==null){
            charging_pads[ind]= drone;
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


    public Drone releaseChargingDrone(int ind){
        Drone res = charging_pads[ind];
        charging_pads[ind]= null;
        return res;
    }

    // public GameObject releaseNonChargingObject(int ind){
    //     GameObject res = non_charging_pads[ind];
    //     non_charging_pads[ind]= null;
    //     return res;
    // }


    public int openChargingPad(){
        for (int i=0; i<charging_pads.Length; i++){
            if (charging_pads[i]==null){
                return i;
            }
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


    public bool startupChargingDrone(int ind){
        if (charging_pads[ind]!=null){
            charging_pads[ind].current_state= State.TakeOff; // TODO: Change to commision
            return true;
        }
        return false;
    }

}
