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
        base.Start();

        GameObject drone1 = Instantiate(
            delivary_drone_prefab, 
            this.transform.position + new Vector3(0, 105, 0),
            Quaternion.identity
        );
        pad.holdChargingDrone(drone1, 0);

        jobs_queue.Add(new Job(
            new Vector3(1000f, 0f, 290f),
            JobType.Scan,
            1,
            100
        ));

        jobs_queue.Add(new Job(
            new Vector3(1000f, 0f, 0f),
            JobType.Delivary,
            1,
            100,
            0,
            new Vector3(1000f, 0f, 290f)
        ));

        commisionDrone(0);
    }

    void Update()
    {
        base.Update();
        pad.Update();
    }


    // Utilitys
    public void commisionDrone(int ind){
        GameObject drone = pad.releaseChargingDrone(0);
        drone.GetComponent<Drone>().parked = false;
        drone.GetComponent<Drone>().charging = false;

        drone.GetComponent<Drone>().jobs_queue.Add(jobs_queue[0]);
        drone.GetComponent<Drone>().jobs_queue.Add(jobs_queue[1]);
    }

    // public (List<Job>, float) jobSubsetGeneration(double will){

    // }
}
