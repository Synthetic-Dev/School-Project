using System;
using System.Collections.Generic;
using UnityEngine;

public class Planet : AbstractBody {
    public Constants.PlanetType planetType;

    private void Start() {
        bodyType = Constants.BodyType.Planet;
    }

    // All equations for U(0 - 1) relations can be found in this paper:
    // https://lup.lub.lu.se/luur/download?fileOId=8870454&func=downloadFile&recordOId=8867455
    // I have made some alterations for planets when it comes to the equations given in
    // the above paper, along with using equations given in this blog:
    // https://worldbuildingpasta.blogspot.com/2019/10/an-apple-pie-from-scratch-part-ivb.html

    // Calculates the radius of the planet
    public override double GetRadius() {
        double earthMass = GetMass() * Constants.solarMassToEarthMass;
        double earthRadius = 1;

        if (earthMass < 2.04) {
            earthRadius = 1.008 * Math.Pow(earthMass, 0.279);
        } else if (earthMass < 132) {
            earthRadius = 0.808 * Math.Pow(earthMass, 0.589);
        } else {
            earthRadius = 17.745 * Math.Pow(earthMass, -0.044);
        }

        return earthRadius;
    }

    // Calculates the mass of the planet from the seed and whether it is an inner or outer planet
    public override double GetMass() {
        double mass = 1;
        switch (planetType)
        {
            case Constants.PlanetType.Inner:
                mass = 1e-6 * (2.53 + 20 * Math.Pow((double)seed - 0.5, 3) + 2 * Math.Pow((double)seed, 2));
                break;
            case Constants.PlanetType.Outer:
                mass = 1e-6 * (625 + 5 * Math.Pow(10 * ((double)seed - 0.5), 3));
                break;
        }
        return mass;
    }

    // Returns a new material object used to create the appearance of the object
    public override Material GetMaterial() {
        // Create the colour object from the calculated rgb values
        Color color = UnityEngine.Random.ColorHSV();

        // Instantiate the material object
        Material material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
        material.SetColor("_Color", color);
        material.SetColor("_BaseColor", color);

        // material.EnableKeyword("_SPECULAR_COLOR");

        // material.EnableKeyword("_EMISSION");
        // Color emissionColor = color;
        // material.SetColor("_EmissionColor", emissionColor);

        return material;
    }

    public override Dictionary<string, string> GetInfo() {
        // Call the super class' method so that the dictionary includes the default information
        Dictionary<string, string> dict = base.GetInfo();
        dict.Add("Planet Type", planetType.ToString());
        dict.Add("Mass", GetMass().ToString() + " Suns");
        dict.Add("Radius", GetRadius().ToString() + " Solar Radii");
        return dict;
    }
}