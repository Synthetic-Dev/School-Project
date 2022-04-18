using System;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour {
    AbstractBody[] bodies;
    public float timeStep = 0.01f;
    public float timeSpeed = 1f;
    public int simSeed;
    static Simulation instance;
    public Camera camera;

    void Awake () {
        Time.fixedDeltaTime = timeStep;
        Spawn(simSeed);
    }

    // Creates a game object named "Star" that has the provided mass, radius, position and initialVelocity
    public GameObject Star(float seed, Vector3 position, Vector3 initialVelocity) {
        // Create the game object and set the position and size
        GameObject body = new GameObject("Star");
        body.transform.position = position;

        // Add a rigidbody to the object allowing it to interact with the physics engine
        Rigidbody rb = body.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add a collider allowing the object to collide with other objects
        SphereCollider collider = body.AddComponent<SphereCollider>();
        collider.radius = 1;

        // Add a mesh filter to hold the data about the object's mesh
        MeshFilter filter = body.AddComponent<MeshFilter>();
        // Add a mesh render that will handle the rendering of the mesh
        MeshRenderer renderer = body.AddComponent<MeshRenderer>();

        // A trail used to visualize the motion of the object
        Material trailMaterial = new Material(Shader.Find("Unlit/Color"));
        trailMaterial.color = Color.white;
        TrailRenderer trail = body.AddComponent<TrailRenderer>();
        trail.time = 60;
        trail.widthCurve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0.1f, -2f, 0f)
        );
        trail.material = trailMaterial;

        // Add the class that handles the movement of the object
        var controller = body.AddComponent<Star>();
        controller.seed = seed;

        body.transform.localScale = (new Vector3(2, 2, 2)) * controller.GetRadius() * Constants.solarScale;

        float mass = controller.GetMass() * Constants.solarMassEmphasis;
        Debug.Log(mass);
        rb.mass = mass;
        controller.SetVelocity(initialVelocity);


        // Create the material that is used to describe how light should interact with the object
        Material material = controller.GetMaterial();
        renderer.sharedMaterial = material;

        // Generate the spherical mesh and add it to the mesh filter
        IcoSphere.Create(body);
        
        return body;
    }

    // Handles the unloading of old simulations and the generation of a new one with the given seed
    public void Spawn (int seed) {
        // The random number generator that is used to randomly generate the simulation
        TimeSpan time = DateTime.UtcNow - new DateTime(1970, 1, 1);
        simSeed = (int)time.TotalSeconds;
        PRNG prng = new PRNG(simSeed);
        int galSeed = 5 * ((10 ^ 9) + (10 ^ 5) + 10) + prng.Range(-4999, 5000) * (10 ^ 6) + prng.Range(-499, 500) * (10 ^ 2) + prng.Range(-4999, 5000);
        int systemSeed = galSeed + 10 ^ 17;
        PRNG systemPRNG = new PRNG(systemSeed);

        for (int i = 0; i < 2; i++) {
            float starSeed = systemPRNG.Value();
            Star(starSeed, new Vector3(200 * i, 0, 0), new Vector3(0, 0, 0));
        }

        camera.transform.position = Vector3.zero + Vector3.up * 150 + Vector3.back * 200;
        camera.transform.LookAt(Vector3.zero);
    }

    void FixedUpdate () {
        bodies = FindObjectsOfType<AbstractBody>();

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].UpdateVelocity(bodies, timeStep * timeSpeed);
        }

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].UpdatePosition(timeStep * timeSpeed);
        }

    }

    public static AbstractBody[] Bodies {
        get {
            return Instance.bodies;
        }
    }

    static Simulation Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<Simulation> ();
            }
            return instance;
        }
    }
}