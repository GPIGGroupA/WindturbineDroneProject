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

        jobs_queue.Add(new Job(
            new Vector3(1000f, 0f, 290f),
            JobType.Delivary,
            1,
            100,
            0,
            new Vector3(1000f, 0f, 0f)
        ));

        drone1.GetComponent<Drone>().jobs_queue = jobSubsetGeneration(drone1.GetComponent<Drone>());

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

    public List<Job> jobSubsetGeneration(Drone drone){
        List<Job> res = new List<Job>();
        float time= 0, perc= 0, dist= 0;

        // Get initial job
        bool marked_flag = false;
        int argmin = 0; float min = float.MaxValue;

        for (int i=0; i<jobs_queue.Count; i++){
            (float dt, float dp, float dd) = drone.JobTPD(jobs_queue[i], drone.transform.position);
            float dr = Utilities.jobRank(jobs_queue[i])*dp;
            bool dm = jobs_queue[i].deadline <= Time.time + dt ? true : false;

            if (min > dr || dm) {
                min = dr;
                argmin = i;

                if (!marked_flag && dm){
                    marked_flag = true;
                }
            }
        }
        res.Add(jobs_queue[argmin]);
        jobs_queue.RemoveAt(argmin);


        bool loop = true;
        while (loop) {

            float[] hueristics = new float[(res.Count+1)*jobs_queue.Count];
            
            for (int i=0; i<=res.Count; i++){
                for (int j=0; j<jobs_queue.Count; j++){

                    res.Insert(i, jobs_queue[j]);
                    (float dt, float dp, float dd) = drone.JobSubsetTPD(res);
                    res.RemoveAt(i);

                    hueristics[i*jobs_queue.Count+j] = Utilities.jobRank(jobs_queue[j])*dp;
                }
            }

            (float m, float am) = Utilities.argMin(hueristics);
            int posInRes = ((int)am) / jobs_queue.Count;
            int minJobIdx = ((int)am) % jobs_queue.Count;

            res.Insert(posInRes, jobs_queue[minJobIdx]);
            (time, perc, dist) = drone.JobSubsetTPD(res);

            if (perc + drone.battery_level_tolerance > drone.battery_percentage) {
                res.RemoveAt(posInRes);
                loop = false;
            }
            else {
                jobs_queue.RemoveAt(minJobIdx);
                if (jobs_queue.Count <= 0){
                    loop = false;
                }
            }

        }

        return res;
    }
}
