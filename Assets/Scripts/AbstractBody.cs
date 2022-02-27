using System.Collections.Generic;
using UnityEngine;

public class AbstractBody : MonoBehaviour {
    public Constants.BodyType bodyType;
    public Vector3 initialVelocity;

    public Vector3 velocity { get; private set; }
    public Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        velocity = initialVelocity;
    }

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

    public void UpdatePosition(float deltaTime) {
        rb.MovePosition(rb.position + velocity * deltaTime);
    }
}