using UnityEngine;

public class CameraController : MonoBehaviour {
    // This is the slowest speed the camera will move, and will start at when moving the camera
    public float startSpeed = 20;
    public float rotationSpeed = 5;

    private float currentSpeed;
    private Vector3 direction;
    private float rotation;

    // We set the starting values, the camera will not be moving
    private void Awake() {
        currentSpeed = 0;
        direction = Vector3.zero;
    }

    // Every render frame, we check for user input and set the camera speed and direction based on the user's current inputs
    private void Update() {
        Vector3 newDirection = Vector3.zero;

        newDirection += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        // TODO Add these controls into unity settings
        if (Input.GetKeyDown(KeyCode.W)) {
            newDirection += Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            newDirection -= Vector3.forward;
        }
        direction = newDirection;

        float newRotation = 0;

        if (Input.GetKeyDown(KeyCode.Q)) {
            newRotation--;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            newRotation++;
        }

        rotation = newRotation;

        if (rotation != newRotation || direction != newDirection) {
            Debug.Log(direction.ToString() + ", " + rotation.ToString());
        }
    }

    // Now we update the position of the camera based on the user inputs that we detected in the previous update method
    private void LateUpdate() {
        transform.Rotate(transform.forward, rotation * rotationSpeed * Time.deltaTime);
        transform.Translate(direction * currentSpeed * Time.deltaTime);
    }
}