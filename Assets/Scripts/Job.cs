using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {Delivary, Scan}

public struct Job
{
    public Vector3? startTurbineID;
    public Vector3 targetTurbineID;
    public JobType jobtype;
    public int priority;
    public float deadline;
    public float weight;

    public Job(Vector3 p_targetTurbineID, JobType p_jobtype, int p_priority, int p_deadline, float p_weight= 0F, Vector3? p_startTurbineID= null)
    {
        startTurbineID= p_startTurbineID;
        targetTurbineID= p_targetTurbineID;
        jobtype= p_jobtype;
        priority= p_priority;
        deadline= p_deadline;
        weight= p_weight;
    }

}
