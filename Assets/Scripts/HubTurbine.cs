using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubTurbine : WindTurbine
{
    // HubTurbine Infomation
    public LandingPad pad = new LandingPad(9, 0);
    public List<Job> jobs_queue = new List<Job>();
    public float workload = 0f;
    private float servicable_range = 8000f;


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
        pad.holdChargingDrone(drone1);

        jobs_queue.Add(new Job(
            new Vector3(1000f, 0f, 290f),
            JobType.Scan,
            1,
            20
        ));

        // jobs_queue.Add(new Job(
        //     transform.position,
        //     JobType.Delivary,
        //     1,
        //     100,
        //     0,
        //     new Vector3(1000f, 0f, 290f)
        // ));

        // jobs_queue.Add(new Job(
        //     new Vector3(1000f, 0f, 290f),
        //     JobType.Delivary,
        //     1,
        //     100,
        //     0,
        //     transform.position
        // ));

        // jobs_queue.Add(new Job(
        //     transform.position,
        //     JobType.Delivary,
        //     2,
        //     100,
        //     0,
        //     new Vector3(1000f, 0f, 290f)
        // ));

        // jobs_queue.Add(new Job(
        //     new Vector3(1000f, 0f, 290f),
        //     JobType.Delivary,
        //     2,
        //     100,
        //     0,
        //     transform.position
        // ));

    }

    void Update()
    {
        base.Update();
        pad.Update();

        int whichdrone = shouldAndWhichDroneToDeploy();
        if (whichdrone != -1){
            pad.charging_pads[whichdrone].GetComponent<Drone>().jobs_queue = jobSubsetGeneration(pad.charging_pads[whichdrone].GetComponent<Drone>());
            commisionDrone(whichdrone);
        }
    }


    // Utilitys
    public float getWorkload(){ // TODO: here
        // Get average and std of all job counts, use percetage likelihood of being abnormal/ also needs to take into account the amount of drones maybe divide
        return 0f;
    }

    public bool shouldShareDronesWithOtherHub(HubTurbine turb){ // TODO: here
        // Get all hubtrubines in 2x servicable range, get maximal workload and compare to own, if large diffrence then send one drone at maximum
        return false;
    }

    public bool commisionDrone(int ind){
        GameObject? t = pad.releaseChargingDrone(ind);

        if (t==null){
            return false;
        }
        else {
            GameObject drone = (GameObject) t;
            drone.GetComponent<Drone>().parked = false;
            drone.GetComponent<Drone>().charging = false;
            return true;
        }
    }

    public int shouldAndWhichDroneToDeploy(){
        if (pad.charging_pads.Count < 1 || jobs_queue.Count < 1){return -1;}
        float dt, dp, dd;

        // Highest will drone
        float[] wills = new float[pad.charging_pads.Count];
        for (int i=0; i<pad.charging_pads.Count; i++){wills[i]= pad.charging_pads[i].GetComponent<Drone>().willingnessToDeploy();}
        (float m, int am) = Utilities.argMax(wills);
        Drone bestdrone = pad.charging_pads[am].GetComponent<Drone>();

        // If marked job then go
        for (int i=0; i<jobs_queue.Count; i++){
            (dt, dp, dd) = bestdrone.JobTPD(jobs_queue[i], bestdrone.transform.position);
            if (Utilities.isJobMarked(jobs_queue[i], bestdrone, dt) && Utilities.pointInRangeOfPoint(transform.position, jobs_queue[i].targetTurbineID, servicable_range)){
                return am;
            }
        }

        List<Job> propsedsubset = jobSubsetGeneration(bestdrone);
        jobSubsetDeGeneration(propsedsubset); // Job subset generation deletes in the queue so make sure to add back

        (dt, dp, dd) = bestdrone.JobSubsetTPD(propsedsubset);

        // Proposed subsets rank value
        float rankValueInv = 0f;
        for (int i=0; i<propsedsubset.Count; i++){rankValueInv+= 1000 - Utilities.jobRank(propsedsubset[i]);}

        if (dp * propsedsubset.Count * rankValueInv > 100000){
            return am;
        }

        return -1;
    }

    public void jobSubsetDeGeneration(List<Job> jobs){
        foreach (Job job in jobs){
            jobs_queue.Add(job);
        }
    }

    public List<Job> jobSubsetGeneration(Drone drone){ // TODO: Make sure marked jobs are done first, also within servicable range
        List<Job> res = new List<Job>();
        float time= 0, perc= 0, dist= 0;

        // Get initial job
        bool marked_flag = false;
        int argmin = 0; float min = float.MaxValue;

        for (int i=0; i<jobs_queue.Count; i++){
            (float dt, float dp, float dd) = drone.JobTPD(jobs_queue[i], drone.transform.position);
            bool dm = Utilities.isJobMarked(jobs_queue[i], drone, dt);
            float dr = Utilities.jobRank(jobs_queue[i])*dp;

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


        bool loop = jobs_queue.Count>0;
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
