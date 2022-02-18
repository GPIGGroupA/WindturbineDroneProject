using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LandingPad
{
    public float chargeRate = 0.00001F;
    Drone[] charging_pads;
    Drone[] non_charging_pads;


    public LandingPad(int num_chargepads, int num_nonchargepads){
        charging_pads= new Drone[num_chargepads];
        non_charging_pads= new Drone[num_nonchargepads];
    }



    public void Update(){
        // Charge Drones
        for (int i=0; i<charging_pads.Length; i++){

            if (charging_pads[i]!=null && charging_pads[i].battery_percentage < 100F) {
                charging_pads[i].battery_percentage+= chargeRate;
                if (charging_pads[i].battery_percentage > 100F){
                    charging_pads[i].battery_percentage= 100F;
                }
            }

        }
    }

    public Drone relaseChargingDrone(int ind){
        Drone res= charging_pads[ind];
        charging_pads[ind]= null;
        return res;
    }

    public bool holdChargingDrone(Drone drone, int ind){
        if (charging_pads[ind]!=null){
            charging_pads[ind]= drone;
            return true;
        }
        return false;
    }
}
