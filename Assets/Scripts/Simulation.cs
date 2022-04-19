using System;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour {
    AbstractBody[] bodies;
    public float timeStep = 0.01f;
    public float timeSpeed = 1f;
    public int simSeed;
    public bool generateSeed;
    static Simulation instance;
    public Camera camera;

    void Awake () {
        Time.fixedDeltaTime = timeStep;
        Spawn(simSeed);
    }

    // Handles the unloading of old simulations and the generation of a new one with the given seed
    public void Spawn (int seed) {
        if (generateSeed)
        {
            TimeSpan time = DateTime.UtcNow - new DateTime(1970, 1, 1);
            simSeed = (int)time.TotalSeconds;
        }

        SystemGenerator generator = new SystemGenerator(simSeed);
        generator.Generate();

        camera.transform.position = Vector3.zero + Vector3.up * 600 + Vector3.back * 200;
        camera.transform.LookAt(Vector3.zero);

    }

    void FixedUpdate () {
        // SystemGenerator generator = new SystemGenerator(simSeed);
        // float primarySeed = generator.Generate();
        // if (primarySeed > 0.99) {
        //     Debug.Log(simSeed);
        // }
        // simSeed += 1;

        bodies = FindObjectsOfType<AbstractBody>();

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].UpdateVelocity(bodies, timeStep * timeSpeed);
        }

        for (int i = 0; i < bodies.Length; i++) {
            AbstractBody body = bodies[i];
            body.UpdatePosition(timeStep * timeSpeed);

            if (body.transform.position.magnitude > 1e6) {
                Destroy(body);
            }

            float distance = (body.transform.position - camera.transform.position).magnitude;
            float scale = Mathf.Clamp(distance / 500, 1, 250);

            TrailRenderer trail = body.GetComponent<TrailRenderer>();
            trail.widthCurve = new AnimationCurve(
                new Keyframe(0, scale),
                new Keyframe(1, 0f, scale * -2f, 0f)
            );

            if (body.GetType() == typeof(Star)) {
                GameObject lightObj = body.transform.Find("Light").gameObject;
                // Light 
                lightObj.transform.LookAt(camera.transform);
            }
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