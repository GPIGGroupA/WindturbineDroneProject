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
    public GameObject dronePackage;

    // Wind Object
    private WindScript windScript;


    // Drone Behavouir Parameters
    public static float battery_level_tolerance= 10F;
    private static float start_slowdown_dist = 100F;
    private static float goto_precision_dist = 30F;
    private static float goto_precision_vel = 1f;


    // Unity Infomation
    private static float max_speed = 44f;
    private static float max_deltaforce = 1f;
    private static float turnspeed = Mathf.PI;
    private Vector3 velocity;
    private float angle = 0f;

    void UnityMove(Vector3 targetDirection, float deltaForcePerc, bool faceTarget, Vector3 lookatDirection){
        // Coords calc
        Vector3 nextFrameVelocity = Vector3.ClampMagnitude(
            velocity + Vector3.Normalize(targetDirection)*(max_deltaforce*deltaForcePerc), 
            max_speed
        );
        velocity = nextFrameVelocity;
        transform.position = transform.position + nextFrameVelocity*Time.deltaTime;

        // Angle in terms of target direction and wind
        Vector2 wind = windScript.getWindDir(transform.position.x, transform.position.z);
        Vector3 midVec = (new Vector3(targetDirection.x, 0f, targetDirection.z) + new Vector3(wind.x, 0, wind.y)).normalized;

        float targetangle= Mathf.Clamp(deltaForcePerc * (Mathf.PI/4), 0f, Mathf.PI/4);
        if (Vector3.Angle(targetDirection, Vector3.up) <= 15f || Vector3.Angle(targetDirection, Vector3.down) <= 15f){targetangle = 0f;}
        angle += (targetangle - angle) > Time.deltaTime ? Time.deltaTime : (targetangle - angle);


        if (midVec.magnitude > 0f){
            Vector3 nextDirectionToLook = Vector3.RotateTowards(new Vector3(transform.forward.x, 0f, transform.forward.z), new Vector3(lookatDirection.x, 0f, lookatDirection.z), turnspeed*Time.deltaTime, 0f);
            Vector3 relativeup = Vector3.RotateTowards(midVec, Vector3.up, (Mathf.PI/2 - angle), 0.0f);

            Quaternion lookatdirectionrotation = Quaternion.LookRotation(
                !faceTarget ? Vector3.RotateTowards(nextDirectionToLook, Vector3.Reflect(relativeup, Vector3.up), angle, 0f) : nextDirectionToLook,
                !faceTarget ? relativeup : Vector3.Reflect(relativeup*-1f, Vector3.up)
            );
            
            // Quaternion targetdirectionrotation = Quaternion.LookRotation(
            //     Vector3.RotateTowards(midVec, Vector3.down, angle, 0.0f),
            //     Vector3.up
            // );

            transform.rotation = lookatdirectionrotation;
            // transform.eulerAngles = new Vector3(targetdirectionrotation.eulerAngles.x, lookatdirectionrotation.eulerAngles.y, targetdirectionrotation.eulerAngles.z);
        }

        // Direction calculation
        // Vector3 nextDirectionToLook = Vector3.RotateTowards(new Vector3(transform.forward.x, 0f, transform.forward.z), new Vector3(lookatDirection.x, 0f, lookatDirection.z), turnspeed*Time.deltaTime, 0f);
    }

    // void UnityMove(Vector3 targetDirection, float deltaForcePerc, bool faceTarget){
    //     // Coords calc
    //     Vector3 nextFrameVelocity = Vector3.ClampMagnitude(
    //         velocity + Vector3.Normalize(targetDirection)*(max_deltaforce*deltaForcePerc), 
    //         max_speed
    //     );
    //     velocity = nextFrameVelocity;
    //     transform.position = transform.position + nextFrameVelocity*Time.deltaTime;

    //     // Wind direction
    //     Vector2 wind = windScript.getWindDir(transform.position.x, transform.position.z);
    //     // Vector2 reflectedWindDirection = Vector2.Reflect(wind, targetDirection);

    //     // Mid point of reflected wind and where we want to go
    //     Vector3 midVec = targetDirection; //(targetDirection*2 + new Vector3(wind.x, 0, wind.y)).normalized;

    //     // Roll and pitch
    //     // Vector3 targetnextDronePR = midVec;
    //     // if (Vector3.Angle(midVec, Vector3.down)<=90f){
    //     //     targetnextDronePR = Vector3.Reflect(midVec*-1.0f, new Vector3(midVec.x, 0, midVec.z)).normalized;
    //     // }
    //     // float angle= ((float) Mathf.Deg2Rad * Vector3.Angle(targetnextDronePR, Vector3.up) * (1-deltaForcePerc)*Mathf.PI/2) + Mathf.PI/4;

    //     float angle= Mathf.Clamp(deltaForcePerc*(Mathf.PI/2), 0f, Mathf.PI/2);

    //     // targetnextDronePR = Vector3.RotateTowards(targetnextDronePR, Vector3.up, angle, 1.0f);
    //     // Vector3 nextFrameDirection = Vector3.RotateTowards(transform.up, targetnextDronePR, turnspeed*Time.deltaTime, 0.0f);


    //     // transform.up = nextFrameDirection;

    //     // Yaw
    //     Vector3 lookat = new Vector3(targetDirection.x, transform.position.y, targetDirection.z);
    //     if (lookat.magnitude > 0){
    //         transform.rotation = Quaternion.LookRotation(
    //             Vector3.RotateTowards(lookat, Vector3.down, angle, 0.0f), 
    //             Vector3.up
    //         );
    //     }

    //     // transform.rotation = Quaternion.AngleAxis(30, nextFrameDirection);

    //     // transform.RotateAround(transform.position+targetDirection, transform.up, Time.deltaTime * 90f);
    //     // transform.rotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0f, targetDirection.z).normalized, nextFrameDirection);
    // }

    void OnTriggerEnter(Collider collider)
    {
    }

    void OnTriggerExit(Collider collider)
    {
    }

    public void Start(){
        windScript = GameObject.Find("Wind").GetComponent<WindScript>();
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
        velocity = Vector3.Scale(velocity, new Vector3(0.98F, 0.98F, 0.98F));

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
        GoTo(new Vector3(transform.position.x, Util.aviation_plane, transform.position.z));
        return transform.position.y > Util.aviation_plane;
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
        dronePackage.SetActive(false);
        return true;
    }

    bool PickUp(){
        carrying_item= true;
        dronePackage.SetActive(true);
        return true;
    }

    bool Maintain(Vector3 target) // TODO: Make it look pretty
    {
        Vector3 targetDirection = new Vector3(target.x, transform.position.y, target.z) - transform.position;
        Vector3 right = new Vector3(transform.right.x, 0f, transform.right.z);
        Vector3 forward = new Vector3(transform.forward.x, 0f, transform.forward.z);
        UnityMove((right*4+forward+Vector3.up*-1f).normalized, 0.5f, true, targetDirection.normalized);

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
            UnityMove(targetDirection, deltaForcePerc, false, transform.forward);
        }
        else {
            UnityMove(Vector3.up, 0f, false, transform.forward);
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
