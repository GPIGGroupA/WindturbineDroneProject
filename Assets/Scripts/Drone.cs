using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {Parked, TakeOff, StartJob, EndJob, ReturnHome, Land, Failed}


public class Drone : MonoBehaviour {

    // Drone Infomation
    public float battery_percentage= 100F;
    public float battery_level_tolerance= 10F;
    public List<Job> jobs_queue= new List<Job>();


    public State current_state = State.Parked;
    public Job? current_job = null;
    public Vector3 target;
    public bool has_target= false;


    // Unity Infomation
    private float max_speed = 200;
    private float turnspeed = 0.5F;
    Vector3 velocity;


    // Unity things
    void UnityMove(Vector3 targetDirection, float deltaMagnitude, float max_s)
    {
        Vector3 position = transform.position;
        Vector3 nextFrameVelocity = Vector3.ClampMagnitude(velocity + Vector3.Normalize(targetDirection)*deltaMagnitude, max_s);

        velocity = nextFrameVelocity;
        position += nextFrameVelocity * Time.deltaTime;

        transform.position = position;
    }

    void UnityRotateAnimation(Vector3 targetDirection){
        Vector3 targetDroneRotation;
        if (Vector3.Angle(targetDirection, Vector3.down)<=90f){
            Vector3 normal = Vector3.Normalize(targetDirection);
            normal.y = 0;

            targetDroneRotation = Vector3.Reflect(targetDirection*-1.0f, normal);
        } else {
            targetDroneRotation = targetDirection;
        }

        targetDroneRotation = Vector3.Normalize(targetDroneRotation);

        // Make the rotation smaller for realism
        float angle= Vector3.Angle(targetDroneRotation, Vector3.up)*(float) Mathf.Deg2Rad*0.75f;
        targetDroneRotation = Vector3.RotateTowards(targetDroneRotation, Vector3.up, angle, 1.0f);

        Vector3 nextFrameDirection = Vector3.RotateTowards(transform.up, targetDroneRotation, turnspeed*Time.deltaTime, 0.0f);

        transform.up = nextFrameDirection;
    }

    void OnTriggerEnter(Collider collider)
    {
        // if (current_state==State.Land && collider.gameObject.tag == "HubTurbine"){
        //     if (collider.GetComponent<HubTurbine>().pad.holdChargingDrone(this, 2)){
        //         Object.Destroy(this.gameObject);
        //     }
        // }
    }

    void OnTriggerExit(Collider collider)
    {
    }


    // Control things
    public void Start()
    {
    }

    public void Update() 
    {
        StateCheck();
        StateAct();

        battery_percentage-= 0.001F;
        velocity = Vector3.Scale(velocity, new Vector3(0.99F, 0.99F, 0.99F));
    }

    void StateCheck()
    {
        // Specfic state things
        switch (current_state){
            case State.Parked:
                if (jobs_queue.Count!=0){current_state= State.TakeOff;}
                break;

            case State.TakeOff:
                if (transform.position.y>400f){
                    if (whichJobToDo(transform.position)!=null){current_state= State.StartJob;}
                    else {current_state= State.ReturnHome;}
                }
                break;

            case State.StartJob:
                current_state= State.EndJob;
                break;

            case State.EndJob:
                if (whichJobToDo(transform.position)!=null){current_state= State.StartJob;}
                else {current_state= State.ReturnHome;}
                break;

            case State.ReturnHome:
                if ((transform.position - target).magnitude < 30f){
                    current_state= State.Land;
                }
                break;

            case State.Land:
                // Will be handled with colliders
                break;
        }

        // Allowed in all states
        if (estimatedBatteryCostForSafeReturn() + battery_level_tolerance >= battery_percentage){
            current_state= State.ReturnHome;
        }
        if (transform.position.y<=0f){
            current_state= State.Failed;
        }
    }

    void StateAct()
    {
        switch(current_state){
            case State.Parked:
                Parked();
                break;

            case State.TakeOff:
                TakeOff();
                break;

            case State.StartJob:
                StartJob();
                break;

            case State.EndJob:
                EndJob();
                break;

            case State.ReturnHome:
                ReturnHome();
                break;

            case State.Land:
                Land();
                break;

            case State.Failed:
                Failed();
                break;
        }
    }


    // State things
    void Parked()
    {}

    void TakeOff()
    {
        UnityMove(Vector3.up, 1F, max_speed);
        UnityRotateAnimation(Vector3.up);
    }

    void StartJob()
    {}

    void EndJob()
    {}

    void ReturnHome()
    {
        Move();
    }

    void Land()
    {
        UnityMove(Vector3.down, 1F, max_speed);
        UnityRotateAnimation(Vector3.down);
    }

    void Failed()
    {
        if (transform.position.y > 0F){
            UnityMove(Vector3.down, 10F, 10000F);
        }
    }




    // Utilitys
    public Job? whichJobToDo(Vector3 startpoi){
        if (jobs_queue.Count!=0){
            return jobs_queue[0];
        }
        return null;
    }

    void Move()
    {
        if (has_target){
            Vector3 targetDirection = target - transform.position;
            float force= Mathf.Clamp(targetDirection.magnitude/400, 0, 1);

            if (force < 0.01f){
                UnityMove(Vector3.zero, 1F, max_speed);
            } else {
                UnityMove(targetDirection, force, max_speed);
            }

            UnityRotateAnimation(Vector3.RotateTowards(targetDirection, Vector3.up, (1-force)*(Mathf.PI/2), 1.0f));
        }
        else {
            UnityMove(Vector3.zero, 0F, max_speed);
            UnityRotateAnimation(Vector3.up);
        }
    }

    // Heuristics
    public float estimatedBatteryCostForJob(Job job, Vector3 startpoi){
        return 0.0F;
    }

    public float estimatedBatteryCostForSafeReturn(){
        return 10f;
    }

}
