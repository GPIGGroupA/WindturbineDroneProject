using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionType {GoTo, Land, TakeOff};
public struct Action {
    public ActionType action;
    public Vector3? target;

    public Action(ActionType p_action, Vector3? p_target= null){
        action = p_action;
        target = p_target;
    }
}


public class Drone : MonoBehaviour {

    // Drone Infomation
    public float battery_percentage= 100F;
    public float battery_level_tolerance= 10F;
    public List<Job> jobs_queue= new List<Job>();

    public List<Action> action_stack = new List<Action>();


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
        action_stack.Add(new Action( ActionType.TakeOff));
        action_stack.Add(new Action( ActionType.GoTo, new Vector3(0f, 400f, 0f)));
        action_stack.Add(new Action( ActionType.Land));
    }

    public void Update() 
    {
        if (action_stack.Count != 0){

            bool ret= false;
            switch(action_stack[0].action){

                case ActionType.GoTo:
                    ret = GoTo((Vector3) action_stack[0].target);
                    break;

                case ActionType.TakeOff:
                    ret = TakeOff();
                    break;

                case ActionType.Land:
                    ret = Land();
                    break;

            }

            if (ret){action_stack.RemoveAt(0);}
        
        }


        battery_percentage-= 0.001F;
        velocity = Vector3.Scale(velocity, new Vector3(0.99F, 0.99F, 0.99F));
    }


    // Actions
    bool GoTo(Vector3 target)
    {
        Move(target);
        return (target-transform.position).magnitude < 30f;
    }

    bool TakeOff()
    {
        Vector3 above = transform.position;
        above.y= 400f;

        return GoTo(above);
    }

    bool Land()
    {
        Vector3 below = transform.position;
        below.y = 10f;

        return GoTo(below);
    }




    // Utilitys
    void Move(Vector3? target=null)
    {
        if (target!=null){
            Vector3 targetDirection = (Vector3) target - transform.position;
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

    public Job? whichJobToDo(Vector3 startpoi){
        if (jobs_queue.Count!=0){
            return jobs_queue[0];
        }
        return null;
    }

    // Heuristics
    public float estimatedBatteryCostForJob(Job job, Vector3 startpoi){
        return 0.0F;
    }

    public float estimatedBatteryCostForSafeReturn(){
        return 10f;
    }

}
