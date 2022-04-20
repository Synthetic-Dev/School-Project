using System;
using UnityEngine;
using UnityEngine.UI;

public class NewSim : MonoBehaviour {
    private Simulation sim;
    private GameObject startMenu;
    private GameObject hudMenu;

    private void Awake() {
        startMenu = transform.parent.Find("Start").gameObject;
        hudMenu = transform.parent.Find("HUD").gameObject;
        GameObject simObj = GameObject.Find("Simulation");
        sim = simObj.GetComponent<Simulation>();

        GameObject error = transform.Find("Error").gameObject;
        Text errorText = error.GetComponent<Text>();
        errorText.text = "";
    }

    public void SetActive(bool active) {
        gameObject.SetActive(active);
        startMenu.SetActive(!active);
    }

    private void SetSeed(int seed) {
        GameObject inputObject = transform.Find("Seed").gameObject;
        InputField input = inputObject.GetComponent<InputField>();
        input.text = seed.ToString();

        sim.simSeed = seed;
    }

    public void Generate() {
        InputEnded();
        gameObject.SetActive(false);

        sim.Spawn();

        hudMenu.SetActive(true);
    }

    public void Randomize() {
        TimeSpan time = DateTime.UtcNow - new DateTime(1970, 1, 1);
        PRNG prng = new PRNG((int)time.TotalSeconds);

        SetSeed(prng.NextInt());
    }

    public void InputEnded() {
        GameObject inputObject = transform.Find("Seed").gameObject;
        InputField input = inputObject.GetComponent<InputField>();

        int seed;
        bool success = int.TryParse(input.text, out seed);

        GameObject error = transform.Find("Error").gameObject;
        Text errorText = error.GetComponent<Text>();
        if (!success) {
            seed = 0;

            errorText.text = "Simulation seed needs to be an integer!";
        } else {
            errorText.text = "";
        }
        SetSeed(seed);
    }
}