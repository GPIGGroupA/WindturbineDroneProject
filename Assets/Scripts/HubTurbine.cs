using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HubTurbine : WindTurbine
{
    // HubTurbine Infomation
    public LandingPad pad = new LandingPad(9, 0);
    public List<Job> jobs_queue = new List<Job>();


    // Unity Infomation
    public GameObject delivary_drone_prefab;
    public GameObject maintenance_drone_prefab;

    
    // Control things
    void Start()
    {
        // TODO: Remove
        jobs_queue.Add(new Job("A31", JobType.Scan, 0, 0));

        GameObject drone = Instantiate(
                delivary_drone_prefab, 
                this.transform.position + new Vector3(0, 105, 0), 
                Quaternion.identity
            );
        pad.holdChargingDrone(drone, 0);
    }

    void Update()
    {
        if (shouldDeployNewDrone()){
            commisionDrone(whichDroneToDeply());
        }
        pad.Update();
    }


    // Utilitys
    public void commisionDrone(int ind){
        pad.startupChargingDrone(ind);
    }

    public List<Job> buildJobListForDrone(Drone drone){
        List<Job> res = new List<Job>();

        return res;
    }


    // Heuristics
    public bool shouldDeployNewDrone()
    {
        return true;
    }

    public int whichDroneToDeply()
    {
        return 0;
    }

    public float jobValueBasedOfCurrentJobs(List<Job> currentJobs){
        return 0.0F;
    }
}
