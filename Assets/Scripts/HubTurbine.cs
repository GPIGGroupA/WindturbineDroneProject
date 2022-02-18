using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HubTurbine : WindTurbine
{
    // HubTurbine Infomation
    public LandingPad pad = new LandingPad(9, 0);
    public float chargeRate = 0.00001F;
    public List<Job> jobs_queue = new List<Job>();


    // Unity Infomation
    public GameObject delivary_drone_prefab;
    public GameObject maintenance_drone_prefab;

    
    // Control things
    void Start()
    {
        // TODO: Remove
        jobs_queue.Add(new Job("A31", JobType.Scan, 0, 0));
    }

    void Update()
    {

        if (shouldDeployNewDrone()){
            int i = whichDroneToDeply();
            if (i != -1){
                ReleaseDrone(i);
            }
        }

        pad.Update();
    }


    // Utilitys
    public bool ReleaseDrone(int ind)
    {
        // TODO: Somehow add drone class into the init drone
        Drone res= pad.relaseChargingDrone(ind);

        if (res.GetType() == typeof(DelivaryDrone)){
            Instantiate(
                delivary_drone_prefab, 
                this.transform.position + new Vector3(0, 105, 0), 
                Quaternion.identity
            );
        }
        else if (res.GetType() == typeof(MaintenanceDrone)){
            Instantiate(
                maintenance_drone_prefab, 
                this.transform.position + new Vector3(0, 105, 0), 
                Quaternion.identity
            );
        }
        else {
            return false;
        }

        return true;
    }

    public List<Job> buildJobListForDrone(Drone drone){
        List<Job> res = new List<Job>();

        return res;
    }


    // Heuristics
    public bool shouldDeployNewDrone()
    {
        return false;
    }

    public int whichDroneToDeply()
    {
        return -1;
    }

    public float jobValueBasedOfCurrentJobs(List<Job> currentJobs){
        return 0.0F;
    }
}
