using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionType {GoTo, Land, TakeOff, Maintain};
public struct Action {
    public ActionType action;
    public Vector3? target;

    public Action(ActionType p_action, Vector3? p_target= null){
        action = p_action;
        target = p_target;
    }
}


public class Drone : MonoBehaviour {

    // Drone Control Infomation
    public float battery_percentage= 100F;
    public List<Job> jobs_queue= new List<Job>();
    public List<Action> action_stack = new List<Action>();
    public bool parked = false;
    public bool charging = false;


    // Drone Behavouir Parameters
    private float battery_level_tolerance= 10F;
    private float aviation_plane = 300F;
    private float start_slowdown_dist = 100F;
    private float goto_precision_dist = 30F;
    private float goto_precision_vel = 1f;


    // Unity Infomation
    private float max_speed = 44f;
    private float max_deltaforce = 1f;
    private float turnspeed = 0.5F;
    private Vector3 velocity;


    // Unity things
    void UnityMove(Vector3 targetDirection, float deltaForcePerc)
    {
        Vector3 deltaVelocity = Vector3.Normalize(targetDirection)*(max_deltaforce*deltaForcePerc);
        Vector3 nextFrameVelocity = Vector3.ClampMagnitude(velocity + deltaVelocity, max_speed);

        velocity = nextFrameVelocity;
        transform.position = transform.position + nextFrameVelocity*Time.deltaTime;
    }

    void UnityRotateAnimation(Vector3 targetDirection, float force){
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
        float angle= ((float) Mathf.Deg2Rad * Vector3.Angle(targetDroneRotation, Vector3.up) * (1-force)*Mathf.PI/2) + Mathf.PI/4;
        targetDroneRotation = Vector3.RotateTowards(targetDroneRotation, Vector3.up, angle, 1.0f);

        Vector3 nextFrameDirection = Vector3.RotateTowards(transform.up, targetDroneRotation, turnspeed*Time.deltaTime, 0.0f);

        transform.up = nextFrameDirection;
    }

    void OnTriggerEnter(Collider collider)
    {
    }

    void OnTriggerExit(Collider collider)
    {
    }


    // Control things
    public void Start(){

    }

    public void Update() 
    {
        if (action_stack.Count != 0){ // Things to do on the stack

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

                case ActionType.Maintain:
                    ret = Maintain((Vector3) action_stack[0].target);
                    break;

            }

            if (ret){action_stack.RemoveAt(0);}
        
        }
        else { // Nothing on the stack, put things on the stack
            Job? job = nextJob();
            if (job != null){
                Job tmp = (Job) job;
                switch(tmp.jobtype){

                    case JobType.Scan:
                        action_stack.Add(new Action(ActionType.TakeOff));
                        Vector3 target= tmp.targetTurbineID;
                        target += Vector3.Normalize(target - transform.position)*100;
                        target.y = aviation_plane;
                        action_stack.Add(new Action(ActionType.GoTo, target));
                        action_stack.Add(new Action(ActionType.Maintain, tmp.targetTurbineID));
                        action_stack.Add(new Action(ActionType.TakeOff));
                        break;

                }
                jobs_queue.Remove(tmp);
            }
            else {
                // Return to base
                action_stack.Add(new Action(ActionType.TakeOff));
                action_stack.Add(new Action(ActionType.GoTo, closestHubTurbine(transform.position)));
                action_stack.Add(new Action(ActionType.Land));
            }
        }

        battery_percentage-= 0.001F;
        velocity = Vector3.Scale(velocity, new Vector3(0.99F, 0.99F, 0.99F));

    }


    // Actions
    bool GoTo(Vector3 target)
    {
        Move(target);
        return (target-transform.position).magnitude < goto_precision_dist && velocity.magnitude < goto_precision_vel;
    }

    bool TakeOff()
    {
        Vector3 above = transform.position;
        above.y= aviation_plane;

        return GoTo(above);
    }

    bool Land()
    {
        Vector3 below = transform.position;
        below.y = 0f;

        return GoTo(below);
    }

    bool Maintain(Vector3 target)
    {
        Vector3 height_target= target;
        height_target.y = transform.position.y;

        transform.forward = Vector3.RotateTowards(transform.forward, height_target-transform.position, Mathf.PI/256, 1.0f);
        UnityMove(transform.right+transform.forward+(Vector3.up*-0.4f), 1F);

        if (transform.position.y < 20f){
            return true;
        }
        return false;
    }


    // Utilitys
    void Move(Vector3? target=null)
    {
        if (target!=null){
            Vector3 targetDirection = (Vector3) target - transform.position;
            float deltaForcePerc= Mathf.Clamp(targetDirection.magnitude/start_slowdown_dist, 0, 1);

            UnityMove(targetDirection, deltaForcePerc);
            UnityRotateAnimation(
                targetDirection,
                deltaForcePerc
            );
        }
        else {
            UnityMove(Vector3.zero, 0F);
            UnityRotateAnimation(Vector3.up, 0F);
        }
    }

    public Job? nextJob(){
        if (jobs_queue.Count!=0){
            return jobs_queue[0];
        }
        return null;
    }

    public Vector3 closestHubTurbine(Vector3 startpoi){
        return new Vector3(1000f, aviation_plane, 0f);
    }

    // Heuristics
    public float willingnessToDeploy(){
        return Mathf.Pow(battery_percentage/10, 2);
    }

}
