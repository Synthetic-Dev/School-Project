using System;
using UnityEngine;

public class SystemGenerator {
    public int generatorSeed;

    public SystemGenerator(int seed) {
        generatorSeed = seed;
    }

    private GameObject CreateStar(float seed) {
        // Create the game object and set the position and size
        GameObject body = new GameObject("Star");

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
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

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
        var star = body.AddComponent<Star>();
        star.seed = seed;

        body.transform.localScale = (new Vector3(2, 2, 2)) * (float)(star.GetRadius() * Constants.solarScale);

        double mass = star.GetMass();
        rb.mass = (float)mass;

        // Create the material that is used to describe how light should interact with the object
        Material material = star.GetMaterial();
        renderer.sharedMaterial = material;

        // Generate the spherical mesh and add it to the mesh filter
        IcoSphere.Create(body);

        GameObject lightObject = new GameObject("Light");
        Light light = lightObject.AddComponent<Light>();

        light.type = LightType.Directional;
        light.shadows = LightShadows.Hard;
        light.lightmapBakeType = LightmapBakeType.Realtime;
        light.intensity = (float)Math.Max(3, Math.Pow(2, star.GetIntensity()));
        light.color = material.color;

        lightObject.transform.SetParent(body.transform);

        return body;
    }

    private GameObject CreatePlanet(float seed, float distance) {
        // Create the game object and set the position and size
        GameObject body = new GameObject("Planet");

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

        // Add the class that handles the movement of the object
        var planet = body.AddComponent<Planet>();
        planet.seed = seed;
        if (distance < 800) {
            planet.planetType = Constants.PlanetType.Inner;
        } else {
            planet.planetType = Constants.PlanetType.Outer;
        }

        float radius = (float)(planet.GetRadius() * Constants.solarScale * Constants.planetScale);
        body.transform.localScale = (new Vector3(2, 2, 2)) * radius;

        double mass = planet.GetMass();
        rb.mass = (float)mass;

        // Create the material that is used to describe how light should interact with the object
        Material material = planet.GetMaterial();
        renderer.sharedMaterial = material;

        // A trail used to visualize the motion of the object
        Material trailMaterial = new Material(Shader.Find("Unlit/Color"));
        trailMaterial.color = material.color;
        TrailRenderer trail = body.AddComponent<TrailRenderer>();
        trail.time = 60;
        trail.widthCurve = new AnimationCurve(
            new Keyframe(0, 1f),
            new Keyframe(1, 0f, 1f * -2f, 0f)
        );
        trail.material = trailMaterial;

        // Generate the spherical mesh and add it to the mesh filter
        IcoSphere.Create(body);
        
        return body;
    }

    public void Generate() {
        // Create a new PRNG object using the simulation's seed and generate the value for the primary star
        float primarySeed = (new PRNG(generatorSeed)).Value();

        // Calculate the probabilty that this simulation is a binary star system
        float binaryProbabilty = Mathf.Max(Mathf.Exp(primarySeed - 1f) - 0.37f - Mathf.Pow(primarySeed, 40f), 0.01f);

        // Calculate a new seed that will determine the characteristics of the rest of the system
        int systemSeed = (int)(primarySeed * 1e7f);
        // Generate the value that will determine whether the system is binary
        float binarySeed = (new PRNG(systemSeed)).Value();

        // Create a new game object for the primary star and get the Star component
        GameObject primaryStarObject = CreateStar(primarySeed);
        Star primaryStar = primaryStarObject.GetComponent<Star>();

        // Initialize variables to be used when generating planets
        double maxPlanetDistance = primaryStar.GetRadius() * 10000;
        double previousPlanetDistance;
        double centralMass;

        // Check if a binary system should be generated
        if (binarySeed < binaryProbabilty) {
            // Generate the value for the second star
            float secondarySeed = (new PRNG(systemSeed - 1)).Value();
            // Adjust the value so that the second star will be slightly smaller than the primary star
            secondarySeed = primarySeed * (primarySeed + secondarySeed * (1 - primarySeed));

            // Generate the value to determine the separation between the 2 stars
            float separationSeed = (new PRNG(systemSeed - 2)).Value();
            double separation = primaryStar.GetRadius()
                * ((1 - (double)separationSeed) * 21 + Math.Pow((double)separationSeed, 20) * 1e3);
            double orbitRadius = separation * 0.5f;

            // Create another game object for the second star and get its Star component
            GameObject secondaryStarObject = CreateStar(secondarySeed);
            Star secondaryStar = secondaryStarObject.GetComponent<Star>();

            // Set the central mass of the system to be the sum of both of the star's masses
            centralMass = primaryStar.GetMass() + secondaryStar.GetMass();
            // Calculate the distance to the centre of mass between the 2 stars
            double distanceToCM = (primaryStar.GetMass() * separation) / centralMass;

            // Calculate the initial orbital period and velocity of the two stars
            // They can use the same value since the stars are similar in size
            double orbitPeriod = Calculations.Orbits.GetPeriod(primaryStar.GetMass(), secondaryStar.GetMass(), distanceToCM);
            double velocity = Calculations.Orbits.GetVelocity(primaryStar.GetMass(), secondaryStar.GetMass(), 2 * distanceToCM);

            // (double period, string measure) = Calculations.Time.SecondsToSuitableTime(orbitPeriod);
            // Debug.Log("Binary, Mass: " + centralMass.ToString()
            //     + "SM, Period: " + period.ToString() + measure + ", Velocities: " + velocity.ToString());

            // Set the position of the primary star and its inital velocity
            primaryStar.SetPosition(Vector3.forward * (float)(orbitRadius * Constants.solarScale));
            primaryStar.SetVelocity(Vector3.right * (float)(velocity * Constants.solarScale));
            primaryStar.SetMaxVelocity((float)(velocity * Constants.solarScale));

            // Set the position of the secondary star and its inital velocity
            secondaryStar.SetPosition(Vector3.back * (float)(orbitRadius * Constants.solarScale));
            secondaryStar.SetVelocity(Vector3.left * (float)(velocity * Constants.solarScale));
            secondaryStar.SetMaxVelocity((float)(velocity * Constants.solarScale));

            // This is minimum distance that a planet can generate to the stars
            previousPlanetDistance = 2 * separation;
        } else {
            // Place the star in the centre of the simulation
            primaryStar.SetPosition(Vector3.zero);

            // Set the central mass of the simulation to the mass of the primary star
            centralMass = primaryStar.GetMass();
            // Don't allow any planets to be generated closer than 10 times the radius of the star
            previousPlanetDistance = 10 * primaryStar.GetRadius();
        }

        int planetIndex = 0;
        while (true) {
            planetIndex++;

            float planetSeed = (new PRNG(systemSeed + planetIndex)).Value();
            double planetDistance = Math.Pow(previousPlanetDistance, 1.1)
                - (Math.Log10(Math.Pow((double)planetSeed, 10)) - 1 / 1600) * 10;
            previousPlanetDistance = planetDistance;

            if (planetIndex > 10 || planetDistance >= maxPlanetDistance) {
                break;
            }

            GameObject planetObject = CreatePlanet(planetSeed, (float)planetDistance);
            Planet planet = planetObject.GetComponent<Planet>();

            double orbitCentralMass = (centralMass + planet.GetMass());
            double distanceToCM = (centralMass * planetDistance) / orbitCentralMass;

            double orbitPeriod = Calculations.Orbits.GetPeriod(centralMass, planet.GetMass(), planetDistance);
            double velocity = Calculations.Orbits.GetVelocity(centralMass, planet.GetMass(), planetDistance / Math.PI);

            (double period, string measure) = Calculations.Time.SecondsToSuitableTime(orbitPeriod);
            Debug.Log("Planet, Mass: " + planet.GetMass().ToString()
                + "SM, Distance: " + planetDistance.ToString() + "SR, CM Dist: " + distanceToCM.ToString()
                + "SR, Period: " + period.ToString() + measure + ", Velocity: " + velocity.ToString());

            planet.SetPosition(Vector3.right * (float)(planetDistance * Constants.solarScale));
            planet.SetVelocity(Vector3.back * (float)(velocity * Constants.solarScale));
        }
    }
}