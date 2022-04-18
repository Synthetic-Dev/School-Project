using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBody : MonoBehaviour {
    public Constants.BodyType bodyType;

    public Vector3 velocity { get; private set; }
    public Rigidbody rb;
    public float seed;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Allows other objects to set the velocity of this object
    public void SetVelocity(Vector3 vel) {
        velocity = vel;
    }

    // Update current internal velocity using F=GMm/r^2
    public void UpdateVelocity(AbstractBody[] bodies, float deltaTime) {
        foreach (var body in bodies) {
            if (body != this) {
                Vector3 separation = body.rb.position - rb.position;

                Vector3 acceleration = separation.normalized * Constants.gravitationalConstant
                    * (body.rb.mass / separation.sqrMagnitude);
                velocity += acceleration * deltaTime;
            }
        }
    }

    // Update the position of linked game object based on current position and velocity
    public void UpdatePosition(float deltaTime) {
        rb.MovePosition(rb.position + velocity * deltaTime);
    }

    // Abstract methods that must be defined by a child class
    public abstract float GetRadius();
    public abstract float GetMass();
    public abstract Material GetMaterial();

    // A method used by the user interface to get the information to display to the user
    public virtual Dictionary<string, string> GetInfo() {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("Type", bodyType.ToString());
        return dict;
    }
}