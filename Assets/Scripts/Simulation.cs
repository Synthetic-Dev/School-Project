using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour {
    AbstractBody[] bodies;
    public float timeStep = 0.01f;
    static Simulation instance;
    public Camera camera;

    void Awake () {
        Time.fixedDeltaTime = timeStep;
        Debug.Log("hi");
        Spawn(0);
    }

    public GameObject sun(int mass, int radius, Vector3 pos) {
        GameObject body = new GameObject("Sun");
        body.transform.position = pos;
        body.transform.localScale = (new Vector3(2, 2, 2)) * radius;

        Rigidbody rb = body.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = false;
        SphereCollider collider = body.AddComponent<SphereCollider>();
        collider.radius = 1;

        MeshFilter filter = body.AddComponent<MeshFilter>();
        MeshRenderer renderer = body.AddComponent<MeshRenderer>();

        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        renderer.material = material;
        material.SetColor("_BaseColor", Random.ColorHSV());

        IcoSphere.Create(body);

        body.AddComponent<AbstractBody>();
        return body;
    }

    public void Spawn (int seed) {
        PRNG prng = new PRNG(seed);

        sun(10000000, 10, new Vector3(50, 0, 0));
        var body = sun(10000, 10, new Vector3(-50, 0, 0));
        camera.transform.position = body.transform.position + Vector3.back * 50;
    }

    void FixedUpdate () {
        bodies = FindObjectsOfType<AbstractBody>();

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].UpdateVelocity(bodies, timeStep);
        }

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].UpdatePosition(timeStep);
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