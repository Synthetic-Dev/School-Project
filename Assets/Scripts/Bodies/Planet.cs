using System.Collections.Generic;
using UnityEngine;

public class Planet : AbstractBody {
    private void Start() {
        bodyType = Constants.BodyType.Planet;
    }

    // All equations for U(0 - 1) relations can be found in this paper:
    // https://lup.lub.lu.se/luur/download?fileOId=8870454&func=downloadFile&recordOId=8867455

    // Calculates the luminosity of the star
    public float GetLuminosity() {
        return Mathf.Min(0.00016f + 45f * Mathf.Pow(-Mathf.Log10(1f - seed) / 4.6f, 5.4f), 78100000f);
    }

    // Calculates the radius of the star
    public override float GetRadius() {
        return Mathf.Min(0.08f - 0.43912f * Mathf.Log10(1f - seed), 12);
    }

    // Calculates the mass of the star based on its luminosity
    public override float GetMass() {
        float lum = GetLuminosity();
        float a = 3200f, b = 1f;

        if (lum < 0.03) {
            a = 0.23f;
            b = 2.3f;
        } else if (lum < 16) {
            a = 1f;
            b = 4f;
        } else if (lum < 54666) {
            a = 1.5f;
            b = 3.5f;
        }

        return Mathf.Pow(lum / a, b);
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

        float intensity = Mathf.Clamp(GetLuminosity(), 1f, 7f);
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