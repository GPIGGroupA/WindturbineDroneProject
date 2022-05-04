using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubTurbine : WindTurbine
{
    // Hub Infomation
    public LandingPad pad = new LandingPad(9);
    public List<Job> jobs_queue = new List<Job>();

    // Unity Infomation
    public GameObject delivary_drone_prefab;
    public GameObject maintenance_drone_prefab;

    
    void Start()
    {
        base.Start();

        GameObject drone1 = Instantiate(
            delivary_drone_prefab,
            new Vector3(transform.position.x, transform.position.y + 196, transform.position.z - 10),
            Quaternion.identity
        );
        pad.holdChargingDrone(drone1);

        jobs_queue.Add(
            new Job(
                new Vector3(1000f, 0f, 290f),
                JobType.Delivary,
                1,
                20,
                0,
                transform.position
            )
        );

    }

    void Update()
    {
        base.Update();
        pad.Update();

        // Do we deploy a drone and if so what jobs from the jobqueue
        int whichdrone = shouldAndWhichDroneToDeploy();
        if (whichdrone != -1){
            commisionDrone(
                whichdrone, 
                jobSubsetGeneration(pad.charging_pads[whichdrone].GetComponent<Drone>())
            );
        }
    }


    public bool commisionDrone(int ind, List<Job> jobs){
        GameObject? t = pad.releaseChargingDrone(ind);
        if (t==null){return false;}

        GameObject drone = (GameObject) t;
        drone.GetComponent<Drone>().charging = false;
        drone.GetComponent<Drone>().jobs_queue = jobs;
        return true;
    }

    public int shouldAndWhichDroneToDeploy(){
        float dt, dp, dd;

        if (pad.charging_pads.Count >= 1 && jobs_queue.Count >= 1){

            // Get willingness to deploy values and find max
            float[] wills = new float[pad.charging_pads.Count];
            for (int i=0; i<pad.charging_pads.Count; i++){wills[i]= pad.charging_pads[i].GetComponent<Drone>().willingnessToDeploy();}
            (float m, int am) = Util.max(wills);

            // Drone with highest willingness to deploy
            Drone maxwilldrone = pad.charging_pads[am].GetComponent<Drone>();

            // If any job is marked (Needs to be done now), then deploy
            foreach (Job job in jobs_queue){
                (dt, dp, dd) = maxwilldrone.JobTPD(job, maxwilldrone.transform.position);
                if (job.marked(dt) && Util.distanceToPoint(transform.position, job.targetTurbineID) < Util.hubturbine_servicable_range){
                    return am;
                }
            }

            // If no jobs are marked then see if we have enough value to sending out a drone
            List<Job> propsedsubset = jobSubsetGeneration(maxwilldrone);
            foreach (Job job in propsedsubset){jobs_queue.Add(job);} // Subset destorys so add back in, as this is therotical

            // Proposed subsets total inverted rank value
            float rankValueInv = 0f;
            foreach (Job job in propsedsubset) {rankValueInv+= 1000 - job.rank();}

            // Is proposed subset valuable enough to execute
            (dt, dp, dd) = maxwilldrone.JobSubsetTPD(propsedsubset);
            if (dp * propsedsubset.Count * rankValueInv > 100000){
                return am;
            }

        }
        return -1;
    }

    public List<Job> jobSubsetGeneration(Drone drone){ // TODO: Make sure marked jobs are done first, also within servicable range
        List<Job> res = new List<Job>();
        float time= 0, perc= 0, dist= 0;

        // Get initial job
        bool marked_flag = false;
        int argmin = 0; float min = float.MaxValue;

        for (int i=0; i<jobs_queue.Count; i++){
            (float dt, float dp, float dd) = drone.JobTPD(jobs_queue[i], drone.transform.position);
            bool dm = jobs_queue[i].marked(dt);
            float dr = jobs_queue[i].rank()*dp;

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

                    hueristics[i*jobs_queue.Count+j] = jobs_queue[j].rank()*dp;
                }
            }

            (float m, float am) = Util.min(hueristics);
            int posInRes = ((int)am) / jobs_queue.Count;
            int minJobIdx = ((int)am) % jobs_queue.Count;

            res.Insert(posInRes, jobs_queue[minJobIdx]);
            (time, perc, dist) = drone.JobSubsetTPD(res);

            if (perc + Drone.battery_level_tolerance > drone.battery_percentage) {
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
