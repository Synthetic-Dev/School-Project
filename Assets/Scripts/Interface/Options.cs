using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour {
    private Simulation sim;
    private GameObject startMenu;

    private float defaultTimeStep;
    private bool defaultGenPlanets;

    private void Awake() {
        startMenu = transform.parent.Find("Start").gameObject;
        GameObject simObj = GameObject.Find("Simulation");
        sim = simObj.GetComponent<Simulation>();

        defaultTimeStep = sim.timeStep;
        defaultGenPlanets = sim.generatePlanets;
    }

    public void SetActive(bool active) {
        gameObject.SetActive(active);
        startMenu.SetActive(!active);
    }

    public void ResetValue(string name) {
        GameObject option;
        
        switch(name) {
            case "generatePlanets":
                option = transform.Find("Planets").gameObject;
                Toggle toggle = option.GetComponent<Toggle>();

                toggle.isOn = defaultGenPlanets;
                sim.generatePlanets = defaultGenPlanets;
                break;
            case "timeStep":
                option = transform.Find("TimeStep").gameObject;
                InputField input = option.GetComponent<InputField>();

                input.text = defaultTimeStep.ToString();

                sim.SetTimeStep(defaultTimeStep);
                break;
        }
    }

    public void ValueChanged(string name) {
        GameObject option;
        
        switch(name) {
            case "generatePlanets":
                option = transform.Find("Planets").gameObject;
                Toggle toggle = option.GetComponent<Toggle>();

                sim.generatePlanets = toggle.isOn;
                break;
            case "timeStep":
                option = transform.Find("TimeStep").gameObject;
                InputField input = option.GetComponent<InputField>();

                float timeStep;
                bool success = float.TryParse(input.text, out timeStep);
                if (!success) {
                    timeStep = defaultTimeStep;
                }
                input.text = timeStep.ToString();

                sim.SetTimeStep(timeStep);
                break;
        }
    }
}