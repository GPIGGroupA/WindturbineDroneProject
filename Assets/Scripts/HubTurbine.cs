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

    //list of the drones this hub contains
    public List<GameObject> dronesUnderControl;

    
    // Control things
    void Start()
    {
        // TODO: Remove
        jobs_queue.Add(new Job("A31", JobType.Scan, 0, 0));

        GameObject obj = Instantiate(
                delivary_drone_prefab, 
                this.transform.position + new Vector3(0, 105, 0), 
                Quaternion.identity
            );
        pad.holdChargingDrone(obj.GetComponent<Drone>(), 0);
        // TODO: Remove
        jobs_queue.Add(new Job("A31", JobType.Scan, 0, 0));

        /*
        would probably have a start script which tells the hub turbines to create all the drones
        then can store them in a list of drones for reference and knowledge of which ones this
        hub controls

        for now create the drones and store here in this list
        */
        GameObject drone1 = Instantiate(
            delivary_drone_prefab, 
            this.transform.position + new Vector3(0, 105, 0),
            Quaternion.identity
        );

        dronesUnderControl.Add(drone1);

        GameObject drone2 = Instantiate(
            delivary_drone_prefab, 
            this.transform.position + new Vector3(0, 115, 0),
            Quaternion.identity
        );

        dronesUnderControl.Add(drone2);

        pad.holdChargingDrone(dronesUnderControl[0], 0);
        pad.holdChargingDrone(dronesUnderControl[1], 1);
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
