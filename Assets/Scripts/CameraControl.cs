using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    float camSpeed = 0.3f; // speed
    float wheelSpeed = 100f; //wheel speed
    float screenEdgeThickness = 15f; // value of screen edge space that move camera if mouse moved to the edge of the screen
    float oldAltitude; // stored altitude
    float currentAltitude; // current altitude
    float rotateAmount = 15f;
    float rotateSpeed = 15f;

    private Quaternion rotation;

    // Start is called before the first frame update
    void Start()
    {
        rotation = Camera.main.transform.rotation;
        oldAltitude = Terrain.activeTerrain.SampleHeight(transform.position); // oldAltitude == current altitude at the start (not variable currentAltitude)
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * camSpeed; // left/right movement (x axis)
        float moveY = Input.GetAxisRaw("Mouse ScrollWheel"); // vertical movement (y axis)
        float moveZ = Input.GetAxis("Vertical") * camSpeed; // forward/backward movement (x axis)
        float xPos = Input.mousePosition.x; // x position of the mouse
        float yPos = Input.mousePosition.y; // y position of the mouse

        currentAltitude = Terrain.activeTerrain.SampleHeight(transform.position); // sets the altitude value

        if (oldAltitude != currentAltitude)
        {
            AltitudeAdjustment();
            oldAltitude = currentAltitude;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || xPos > 0 && xPos < screenEdgeThickness)
        {
            moveX -= camSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || xPos < Screen.width && xPos > Screen.width - screenEdgeThickness)
        {
            moveX += camSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || yPos < Screen.height && yPos > Screen.height - screenEdgeThickness)
        {
            moveZ += camSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || yPos > 0 && yPos < screenEdgeThickness)
        {
            moveZ -= camSpeed * Time.deltaTime;
        }

        transform.Translate(new Vector3(moveX, Input.GetAxis("Mouse ScrollWheel") * wheelSpeed * Time.deltaTime, moveZ));
        //clamps the vert movement
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 10 + currentAltitude, 100 + currentAltitude), transform.position.z);

        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        if (Input.GetMouseButton(2))
        {
            destination.x -= Input.GetAxis("Mouse Y") * rotateAmount;
            destination.y += Input.GetAxis("Mouse X") * rotateAmount;
        }

        if (destination != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Camera.main.transform.rotation = rotation;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }

    //adjusts the camera height to follow the terrain
    public void AltitudeAdjustment()
    {
        float newHeight = transform.position.y; //stores the camera transform Y pos
        Vector3 pos = transform.position;// stores the camera transform pos

        pos.y = currentAltitude; // sets the y position value equal the current altitude of the camera

        //if the alt is greater than oldAlt
        if (currentAltitude > oldAltitude)
        {
            pos.y += newHeight * Time.deltaTime; // adds the current y pos value to the alt; deltaTime smoothes the following
        }
        else
        {
            pos.y -= newHeight * Time.deltaTime; // subtracts the current y pos value to the alt; deltaTime smoothes the following
        }

        transform.position = pos; // sets the camera's current transform pos to the new height
    }
}
