using System;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour {
    AbstractBody[] bodies;

    public float timeStep = 0.01f;
    public float timeSpeed = 1f;
    public int simSeed;
    public double simTime;

    public bool generatePlanets = true;
    public bool generateSeed = false;
    public bool running = false;
    public bool paused = false;

    static Simulation instance;
    public Camera mainCamera;

    private List<GameObject> simObjects;

    public void SetTimeStep(float step) {
        timeStep = step;
        Time.fixedDeltaTime = timeStep;
    }

    void Awake () {
        SetTimeStep(timeStep);
    }

    // Handles the unloading of old simulations and the generation of a new one with the given seed
    public void Spawn() {
        simTime = 0;
        timeSpeed = 1f;
        paused = false;

        if (generateSeed)
        {
            TimeSpan time = DateTime.UtcNow - new DateTime(1970, 1, 1);
            simSeed = (int)time.TotalSeconds;
        }

        SystemGenerator generator = new SystemGenerator(simSeed, generatePlanets);
        simObjects = generator.Generate();
        running = true;

        mainCamera.transform.position = Vector3.zero + Vector3.up * 600 + Vector3.back * 200;
        mainCamera.transform.LookAt(Vector3.zero);

        SimpleCameraController controller = mainCamera.GetComponent<SimpleCameraController>();
        controller.enabled = true;
    }

    public void End() {
        running = false;
        paused = false;
        simTime = 0;
        timeSpeed = 1f;

        for (int i = 0; i < simObjects.Count; i++) {
            GameObject gameObj = simObjects[i];
            DestroyImmediate(gameObj);
        }
    }

    void FixedUpdate () {
        if (!running || paused) {
            return;
        }

        bodies = FindObjectsOfType<AbstractBody>();

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].UpdateVelocity(bodies, timeStep * timeSpeed);
        }

        simTime += timeStep * timeSpeed;

        for (int i = 0; i < bodies.Length; i++) {
            AbstractBody body = bodies[i];
            body.UpdatePosition(timeStep * timeSpeed);

            if (body.transform.position.magnitude > 1e6) {
                Destroy(body);
            }

            TrailRenderer trail = body.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                float distance = (body.transform.position - mainCamera.transform.position).magnitude;
                float scale = Mathf.Clamp(distance / 500, 1, 250);

                trail.widthCurve = new AnimationCurve(
                    new Keyframe(0, scale),
                    new Keyframe(1, 0f, scale * -2f, 0f)
                );
            }

            if (body.GetType() == typeof(Star)) {
                GameObject lightObj = body.transform.Find("Light").gameObject;
                // Light 
                lightObj.transform.LookAt(mainCamera.transform);
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