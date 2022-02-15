using UnityEngine;

[AddComponentMenu("Camera-Control/CameraMovement")]
public class CameraMovement : MonoBehaviour {
    
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 6F;
    public float sensitivityY = 6F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -360F;
    public float maximumY = 360F;
    float rotationY = 0F;  
    public float camSpeed = 500f;
    

    void Update ()
    {
        if (Input.GetKey(KeyCode.Mouse1)){
            MouseRotation();
        }
        ArrowMovement();
    }
    
    void Start ()
    {
    }

    void ArrowMovement() {
        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {direction = Vector3.forward;}
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.R)) {direction = Vector3.back;}
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.S)) {direction = Vector3.right;}
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {direction = Vector3.left;}
        if (Input.GetKey(KeyCode.P)) {direction = Vector3.up;}
        if (Input.GetKey(KeyCode.T)) {direction = Vector3.down;}

        transform.Translate(direction * camSpeed * Time.deltaTime);
        
    }

    void MouseRotation() {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
            
            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }
}
