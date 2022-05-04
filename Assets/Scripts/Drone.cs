using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionType {GoTo, Land, TakeOff, Maintain, Deliver, PickUp};
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
    public List<Job> jobs_queue= new List<Job>();
    public List<Action> action_stack = new List<Action>();
    public bool parked = false;
    public bool charging = false;
    public bool carrying_item = false;


    // Drone Behavouir Parameters
    public static float battery_level_tolerance= 10F;
    private static float start_slowdown_dist = 100F;
    private static float goto_precision_dist = 30F;
    private static float goto_precision_vel = 1f;


    // Unity Infomation
    private static float max_speed = 44f;
    private static float max_deltaforce = 1f;
    private static float turnspeed = 0.5F;
    private Vector3 velocity;


    void UnityMove(Vector3 targetDirection, float deltaForcePerc){
        // Coords calc
        Vector3 nextFrameVelocity = Vector3.ClampMagnitude(
            velocity + Vector3.Normalize(targetDirection)*(max_deltaforce*deltaForcePerc), 
            max_speed
        );
        velocity = nextFrameVelocity;
        transform.position = transform.position + nextFrameVelocity*Time.deltaTime;

        // Roll and pitch
        Vector3 nextDronePR = targetDirection;
        if (Vector3.Angle(targetDirection, Vector3.down)<=90f){
            Vector3 normal = Vector3.Normalize(targetDirection);
            nextDronePR = Vector3.Reflect(targetDirection*-1.0f, new Vector3(normal.x, 0, normal.z));
        }
        nextDronePR = Vector3.Normalize(nextDronePR);

        float angle= ((float) Mathf.Deg2Rad * Vector3.Angle(nextDronePR, Vector3.up) * (1-deltaForcePerc)*Mathf.PI/2) + Mathf.PI/4;
        nextDronePR = Vector3.RotateTowards(nextDronePR, Vector3.up, angle, 1.0f);
        Vector3 nextFrameDirection = Vector3.RotateTowards(transform.up, nextDronePR, turnspeed*Time.deltaTime, 0.0f);
        transform.up = nextFrameDirection;
    }

    void OnTriggerEnter(Collider collider)
    {
    }

    void OnTriggerExit(Collider collider)
    {
    }

    public void Start(){

    }

    public void Update() 
    {
        // Take action an the stack if there is one, remove if its completed
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
                    ret = Land((Vector3) action_stack[0].target);
                    break;
                case ActionType.Maintain:
                    ret = Maintain((Vector3) action_stack[0].target);
                    break;
                case ActionType.Deliver:
                    ret = Deliver();
                    break;
                case ActionType.PickUp:
                    ret = PickUp();
                    break;

            }

            if (ret){action_stack.RemoveAt(0);}
        
        }
        // If no actions then
        else {
            Job? job = nextJob();

            // If there is a next job then transpile it into actions
            if (job != null){
                Job tmp = (Job) job;
                Vector3 target;
                switch(tmp.jobtype){

                    case JobType.Scan:
                        action_stack.Add(new Action(ActionType.TakeOff));
                        target= tmp.targetTurbineID;
                        target += Vector3.Normalize(transform.position - target)*150;
                        target.y = Util.aviation_plane;
                        action_stack.Add(new Action(ActionType.GoTo, target));
                        action_stack.Add(new Action(ActionType.Maintain, tmp.targetTurbineID));
                        action_stack.Add(new Action(ActionType.TakeOff));
                        break;

                    case JobType.Delivary:
                        if (!(Util.distanceToPoint(transform.position, tmp.startTurbineID) < 200f && parked)){
                            action_stack.Add(new Action(ActionType.TakeOff));
                            target= (Vector3) tmp.startTurbineID;
                            target.y = Util.aviation_plane;
                            action_stack.Add(new Action(ActionType.GoTo, target));
                            action_stack.Add(new Action(ActionType.Land, target));
                        }
                        action_stack.Add(new Action(ActionType.PickUp));
                        action_stack.Add(new Action(ActionType.TakeOff));
                        target= tmp.targetTurbineID;
                        target.y = Util.aviation_plane;
                        action_stack.Add(new Action(ActionType.GoTo, target));
                        action_stack.Add(new Action(ActionType.Land, target));
                        action_stack.Add(new Action(ActionType.Deliver));
                        action_stack.Add(new Action(ActionType.TakeOff));
                        break;

                }
                jobs_queue.Remove(tmp);
            }
            // Return to base
            else if(!parked) {
                action_stack.Add(new Action(ActionType.TakeOff));
                Vector3 home= (Vector3) Util.locationOfTurbineWithID(Util.closestHubTurbine(transform.position));
                action_stack.Add(new Action(ActionType.GoTo, new Vector3(home.x, Util.aviation_plane, home.z)));
                action_stack.Add(new Action(ActionType.Land, new Vector3(home.x, Util.landing_plane, home.z)));
            }
        }
        velocity = Vector3.Scale(velocity, new Vector3(0.99F, 0.99F, 0.99F));

        if (!charging){
            battery_percentage-= 0.003F;
        }

    }


    // Actions
    bool GoTo(Vector3 target)
    {
        if (!parked){Move(target);}
        return Util.distanceToPoint(target, transform.position) < goto_precision_dist && velocity.magnitude < goto_precision_vel;
    }

    bool TakeOff()
    {
        parked= false;
        return GoTo(new Vector3(transform.position.x, Util.aviation_plane, transform.position.z));
    }

    bool Land(Vector3 target)
    {
        if (GoTo(new Vector3(target.x, Util.landing_plane, target.z))){
            parked = true;
            return true;
        }
        return false;
    }

    bool Deliver(){
        carrying_item= false;
        return true;
    }

    bool PickUp(){
        carrying_item= true;
        return true;
    }

    bool Maintain(Vector3 target) // TODO: Make it look pretty
    {
        Vector3 height_target= target;
        height_target.y = transform.position.y;

        transform.forward = Vector3.RotateTowards(transform.forward, height_target-transform.position, Mathf.PI/16, 1.0f);
        UnityMove(transform.right*4+transform.forward+(Vector3.up*-0.6f), 1F);

        if (transform.position.y < 20f){
            return true;
        }
        return false;
    }


    // Utilitys
    void Move(Vector3? target=null) // TODO: Collision avoidance
    {
        if (target!=null){
            Vector3 targetDirection = (Vector3) target - transform.position;
            float deltaForcePerc= Mathf.Clamp(targetDirection.magnitude/start_slowdown_dist, 0, 1);
            UnityMove(targetDirection, deltaForcePerc);
        }
        else {
            UnityMove(Vector3.up, 0f);
        }
    }

    public Job? nextJob(){
        if (jobs_queue.Count!=0){
            return jobs_queue[0];
        }
        return null;
    }

    public float willingnessToDeploy(){
        return Mathf.Pow(battery_percentage/10, 2);
    }

    public (float time, float perc, float dist) travelpoint2pointTPD(Vector3 a, Vector3 b){
        float dist = (a-b).magnitude;
        float time = dist/38.2f;
        float perc = time*0.18f;
        return (time, perc, dist);
    }

    public (float time, float perc, float dist) gohomeTPD(Vector3 pos){
        float time = 0, perc = 0, dist = 0;
        float dt, dp, dd;

        Vector3 home = (Vector3) Util.locationOfTurbineWithID(Util.closestHubTurbine(pos));

        // GoTo home
        (dt, dp, dd) = travelpoint2pointTPD(pos, new Vector3(home.x, Util.aviation_plane, home.z));
        time+= dt; perc+= dp; dist+= dd;
        pos = new Vector3(home.x, Util.aviation_plane, home.z);

        // Land home
        (dt, dp, dd) = travelpoint2pointTPD(pos, new Vector3(pos.x, Util.landing_plane, pos.z));
        time+= dt; perc+= dp; dist+= dd;

        return (time, perc, dist);
    }

    public (float time, float perc, float dist) JobTPD(Job job, Vector3 poi){
        float time = 0, perc = 0, dist = 0;
        float dt, dp, dd;

        // TakeOff
        (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(poi.x, Util.aviation_plane, poi.z));
        time+= dt; perc+= dp; dist+= dd;
        poi.y = Util.aviation_plane;


        switch (job.jobtype){

            case JobType.Delivary:
                // GoTo pickup point
                (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(((Vector3)job.startTurbineID).x, Util.aviation_plane, ((Vector3)job.startTurbineID).z));
                time+= dt; perc+= dp; dist+= dd;
                poi = new Vector3(((Vector3)job.startTurbineID).x, Util.aviation_plane, ((Vector3)job.startTurbineID).z);

                // Land, Pickup, TakeOff
                (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(poi.x, Util.landing_plane, poi.z));
                time+= dt*2; perc+= dp*2; dist+= dd*2;

                // GoTo dropoff point
                (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(job.targetTurbineID.x, Util.aviation_plane, job.targetTurbineID.z));
                time+= dt; perc+= dp; dist+= dd;
                poi = new Vector3(job.targetTurbineID.x, Util.aviation_plane, job.targetTurbineID.z);

                // Land, DropOff, TakeOff
                (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(poi.x, Util.landing_plane, poi.z));
                time+= dt*2; perc+= dp*2; dist+= dd*2;

                break;

            case JobType.Scan:
                // GoTo scan location point
                (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(job.targetTurbineID.x, Util.aviation_plane, job.targetTurbineID.z));
                time+= dt; perc+= dp; dist+= dd;
                poi = new Vector3(job.targetTurbineID.x, Util.aviation_plane, job.targetTurbineID.z);

                // Scan
                time+= 44f; perc+= 44f*0.18f; dist+= 0f; // TODO: Calculate the distance covered
                poi.y = Util.drone_minimumn_plane;

                // TakeOff
                (dt, dp, dd) = travelpoint2pointTPD(poi, new Vector3(poi.x, Util.aviation_plane, poi.z));
                time+= dt; perc+= dp; dist+= dd;

                break;
        }

        return (time, perc, dist);

    }

    public (float time, float perc, float dist) JobSubsetTPD(List<Job> jobs){
        float time = 0, perc = 0, dist = 0;
        float dt, dp, dd;

        Vector3 simpos = transform.position;
        foreach(Job job in jobs){
            (dt, dp, dd) = JobTPD(job, simpos);
            time+= dt; perc+= dp; dist+= dd;
            simpos = new Vector3(job.targetTurbineID.x, Util.aviation_plane, job.targetTurbineID.z);
        }

        (dt, dp, dd) = gohomeTPD(simpos);
        time+= dt; perc+= dp; dist += dd;

        return (time, perc, dist);
    }

}
