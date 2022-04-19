using System;
using System.Collections.Generic;
using UnityEngine;

public class Star : AbstractBody {
    private void Start() {
        bodyType = Constants.BodyType.Star;
    }

    // All equations for U(0 - 1) relations can be found in this paper:
    // https://lup.lub.lu.se/luur/download?fileOId=8870454&func=downloadFile&recordOId=8867455

    // Calculates the luminosity of the star
    public double GetLuminosity() {
        return Math.Min(0.00016 + 45 * Math.Pow(-Math.Log10(1 - seed) / 4.6, 5.4), 78100000);
    }

    public double GetIntensity() {
        return Calculations.Clamp(GetLuminosity(), 1, 6);
    }

    // Calculates the radius of the star
    public override double GetRadius() {
        return Math.Min(0.08 - 0.43912 * Math.Log10(1 - (double)seed), 12);
    }

    // Calculates the mass of the star
    public override double GetMass() {
        if (seed < 0.9) {
            return -0.9 + Math.Pow(1.02, 1770 * Math.Pow((double)seed - 0.265, 9));
        } else if (seed < 0.942) {
            return -0.8 + Math.Exp(100 * Math.Pow(seed - 0.23, 13.5));
        }
        return Math.Min(-0.73 + Math.Pow(5, 3200 * Math.Pow((double)seed - 0.03, 90)), 70);
    }

    // Returns a new material object used to create the appearance of the object
    public override Material GetMaterial() {
        float red = Mathf.Min(0.362f + Mathf.Pow(-Mathf.Log10(seed), 0.2f), 1f);
        float blue = Mathf.Min(0.25f + 0.9f * Mathf.Pow(-Mathf.Log10(1f - seed) / 4.4f, 0.7f), 1f);

        float green = blue + 0.25f / Mathf.Pow(1f + 2f * seed, 2f);
        if (green > 0.99f) {
            green = 1 - Mathf.Max(Mathf.Pow(seed - 0.95f, 2), Mathf.Pow(seed, 1000) / 3.5f);
        }

        // Create the colour object from the calculated rgb values
        Color color = new Color(red, green, blue);

        // Instantiate the material object
        Material material = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
        material.SetColor("_Color", color);
        material.SetColor("_BaseColor", color);

        material.EnableKeyword("_EMISSION");
        material.EnableKeyword("_RECEIVE_SHADOWS_OFF");
        material.SetFloat("_ReceiveShadows", 0);

        float intensity = (float)GetIntensity();
        float intensityFactor = Mathf.Pow(2, intensity);
        Color emissionColor = new Color(red * intensityFactor, green * intensityFactor, blue * intensityFactor);
        material.SetColor("_EmissionColor", emissionColor);

        return material;
    }

    public override Dictionary<string, string> GetInfo() {
        // Call the super class' method so that the dictionary includes the default information
        Dictionary<string, string> dict = base.GetInfo();
        dict.Add("Mass", GetMass().ToString() + " Suns");
        dict.Add("Radius", GetRadius().ToString() + " Solar Radii");
        dict.Add("Luminosity", GetLuminosity().ToString() + " Solar Luminosity");
        return dict;
    }
}