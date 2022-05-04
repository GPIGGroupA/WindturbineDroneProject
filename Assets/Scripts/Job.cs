using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {Delivary, Scan}

public struct Job
{
    public Vector3 startTurbineID;
    public Vector3 targetTurbineID;
    public JobType jobtype;
    public int priority;
    public float deadline;
    public float weight;

    public Job(Vector3 p_targetTurbineID, JobType p_jobtype, int p_priority, float p_deadline, float p_weight= 0F, Vector3 p_startTurbineID= new Vector3())
    {
        startTurbineID= p_startTurbineID;
        targetTurbineID= p_targetTurbineID;
        jobtype= p_jobtype;
        priority= p_priority;
        deadline= p_deadline;
        weight= p_weight;
    }

    public float rank(){
        return Mathf.Clamp(Mathf.Abs(deadline - Time.time), 0, 100)*priority + priority;
    }

    public bool marked(float predicted_time){
        return deadline <= Time.time + predicted_time;
    }

}
