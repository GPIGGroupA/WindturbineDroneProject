using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // Collect first item with highest value
        Job first_job = jobs_queue.MaxBy(j => jobValue(j));
        jobs_queue.Remove(first_job);

        // Add first item
        List<Job> res = new List<Job>() {first_job};
        float batterycost = drone.estimatedBatteryCostForJob(first_job, drone.transform.position);
        
        // loop whilst the battery cost is below drones battery percentage
        while (batterycost < drone.battery_percentage) {

            // Average working sets coord poisition
            Vector3 average_endpoint = Vector3.zero;
            foreach (Job job in res){average_endpoint += job.targetTurbineID;}
            average_endpoint /= res.Count;


            // Pick maximal job, off weighted sum
                // Job value
                // Distance off course inverted
            Job next_job = jobs_queue.MaxBy(
                j => jobValue(j) + (Utilities.shortestDistanceToLine(drone.transform.position, average_endpoint, j.targetTurbineID) - float.MaxValue)
            );
            jobs_queue.Remove(next_job);
            res.Add(next_job);

            // Calculate route that minimizes travelled distance (TSP problem solver, currently greedy)
            List<Job> ordered_res = new List<Job>() {res[0]};

            for (int i=1; i<res.Count; i++){

                int argmin = -1;
                float min_delta_dist = float.MaxValue;

                for (int j=0; j<ordered_res-1; j++){
                    float deltadist = Mathf.Abs(
                        (ordered_res[j].targetTurbineID - ordered_res[j+1].targetTurbineID).magnitude // Current distance
                        - (ordered_res[j].targetTurbineID - res[i].targetTurbineID).magnitude // New distance
                        + (ordered_res[j+1].targetTurbineID - res[i].targetTurbineID).magnitude
                    );

                    if (deltadist < min_delta_dist){
                        min_delta_dist = deltadist;
                        argmin = j;
                    }
                }

                if (argmin!=-1){ordered_res.Insert(argmin, res[i]);}
                else {ordered_res.Add(res[i]);}

            }

            // Estimate battery cost
            batterycost = drone.estimatedBatteryCostForJob(ordered_res[0], drone.transform.position);
            for (int i=1; i<ordered_res.Count; i++){
                batterycost+= drone.estimatedBatteryCostForJob(ordered_res[i], ordered_res[i-1].targetTurbineID);
            }
            batterycost+= drone.estimatedBatteryCostForSafeReturn(ordered_res[ordered_res.Count-1]);

        }

        // Remove the last item we added within the while loop
        if (res.Count > 1){res.RemoveAt(res.Count - 1);}

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

    public float jobValue(Job job){
        return 0.0F;
    }
}
