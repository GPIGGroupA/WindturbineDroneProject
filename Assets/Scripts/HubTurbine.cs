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
            4,
            100,
            0,
            new Vector3(1000f, 0f, 290f)
        ));

        jobs_queue.Add(new Job(
            new Vector3(1000f, 0f, 290f),
            JobType.Delivary,
            4,
            100,
            0,
            new Vector3(1000f, 0f, 0f)
        ));

        (List<Job> t, float fhf) = jobSubsetGeneration(drone1.GetComponent<Drone>());
        drone1.GetComponent<Drone>().jobs_queue = t;

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

        // drone.GetComponent<Drone>().jobs_queue.Add(jobs_queue[0]);
        // drone.GetComponent<Drone>().jobs_queue.Add(jobs_queue[1]);
        // drone.GetComponent<Drone>().jobs_queue.Add(jobs_queue[2]);
    }

    public (List<Job>, float) jobSubsetGeneration(Drone drone){
        List<Job> res = new List<Job>();

        // Gets intital job gets highest job rank and marked
        Job maxjob = jobs_queue[0];
        bool marked = false;
        float maxjobrank = float.MinValue;
        foreach (Job job in jobs_queue){

            bool dm = false;
            (float dt, float dp) = drone.JobBatteryAndTimeCost(job, drone.transform.position);
            float dr = Utilities.jobRank(job);
            
            // If marked
            if (job.deadline <= Time.time + dt){
                dm = true;
            }

            if (marked && dm && maxjobrank < dr) {
                maxjob = job;
                maxjobrank = dr;
            } else if (!marked && dm) {
                marked = true;
                maxjob = job;
                maxjobrank = dr;
            }
            else if (maxjobrank < dr) {
                maxjob = job;
                maxjobrank = dr;
            }
        }
        res.Add(maxjob);
        jobs_queue.Remove(maxjob);
        (float _, float batcost) = drone.JobSubsetBatteryAndTimeCost(res);


        bool loop = true;
        while (loop){

            float maxheur = float.MinValue;
            int maxpos = 0;

            for (int i=0; i<=res.Count; i++){
                foreach (Job job in jobs_queue){
                    float heur = Utilities.jobRank(job)*Mathf.Clamp(800 - Utilities.shortestDistanceToLine(i==0 ? drone.transform.position : res[i-1].targetTurbineID, i==res.Count ? drone.closestHubTurbine(res[i-1].targetTurbineID) : res[i].targetTurbineID, job.targetTurbineID), 0, 800);

                    if (heur > maxheur){
                        maxjob = job;
                        maxpos = i;
                        maxheur = heur;
                    }

                }
            }

            res.Insert(maxpos, maxjob);
            (_, batcost) = drone.JobSubsetBatteryAndTimeCost(res);

            if (batcost + drone.battery_level_tolerance > drone.battery_percentage){
                res.Remove(maxjob);
                loop = false;
            } else if (jobs_queue.Count<=1){
                loop = false;
                jobs_queue.Remove(maxjob);
            } else {
                jobs_queue.Remove(maxjob);
            }

        }

        return (res, batcost);
    }
}
