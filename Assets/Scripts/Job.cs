using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {Delivary, Scan}

public struct Job
{
    string startTurbineID;
    string targetTurbineID;
    JobType jobtype;
    int priority;
    int deadline;
    float weight;

    public Job(string p_targetTurbineID, JobType p_jobtype, int p_priority, int p_deadline, float p_weight= 0F, string p_startTurbineID= null)
    {
        startTurbineID= p_startTurbineID;
        targetTurbineID= p_targetTurbineID;
        jobtype= p_jobtype;
        priority= p_priority;
        deadline= p_deadline;
        weight= p_weight;
    }

}
