using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum pad_state {Empty, DelivaryDrone, MaintainenceDrone}
public struct ChargingPad
{
    public ChargingPad(
        pad_state p_state= pad_state.Empty, float p_battery_percentage= 0F
    )
    {
        battery_percentage= p_battery_percentage;
        state= p_state;
    }
    public float battery_percentage;
    public pad_state state;
}


public class HubTurbine : WindTurbine
{

    public GameObject delivary_drone_prefab;
    public GameObject maintenance_drone_prefab;
    public ChargingPad[] charging_pads = new ChargingPad[9];
    public float chargeRate = 0.00001F;

    
    // Start is called before the first frame update
    void Start()
    {
        charging_pads[0].state= pad_state.MaintainenceDrone;
        charging_pads[0].battery_percentage = 90F;
    }

    // Update is called once per frame
    void Update()
    {
        
        // Charge Drones
        for (int i=0; i<charging_pads.Length; i++){
            if (
                charging_pads[i].state==pad_state.DelivaryDrone || 
                charging_pads[i].state==pad_state.MaintainenceDrone
            ){
                if (charging_pads[i].battery_percentage < 100F) {
                    charging_pads[i].battery_percentage+= chargeRate;
                    if (charging_pads[i].battery_percentage > 100F){
                        charging_pads[i].battery_percentage= 100F;
                    }
                }
                // TODO: Remove 
                else {
                    ReleaseDrone(i);
                }
            }
        }
    }

    public bool ReleaseDrone(int ind)
    {
        switch (charging_pads[ind].state) 
        {
            case pad_state.DelivaryDrone:
                Instantiate(
                    delivary_drone_prefab, 
                    this.transform.position + new Vector3(0, 105, 0), 
                    Quaternion.identity
                );
                break;

            case pad_state.MaintainenceDrone:
                Instantiate(
                    maintenance_drone_prefab, 
                    this.transform.position + new Vector3(0, 105, 0), 
                    Quaternion.identity
                );
                break;

            default:
                return false;
        }
        
        charging_pads[ind].state= pad_state.Empty;
        return true;
    }

    public bool HoldDrone(Drone drone, int ind)
    {
        if (charging_pads[ind].state != pad_state.Empty){return false;}

        charging_pads[ind].battery_percentage= drone.battery_percentage;
        if (drone.GetType() == typeof(MaintenanceDrone)){
            charging_pads[ind].state= pad_state.MaintainenceDrone;
        }
        else if (drone.GetType() == typeof(DelivaryDrone)){
            charging_pads[ind].state= pad_state.DelivaryDrone;
        }

        return true;
    }
}
