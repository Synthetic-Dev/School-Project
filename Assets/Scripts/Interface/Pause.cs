using UnityEngine;

public class Pause : MonoBehaviour {
    private Simulation sim;
    private GameObject startMenu;
    private GameObject hudMenu;
    private GameObject pauseMenu;

    private bool keyDown = false;
    private bool active = false;

    private void Awake() {
        pauseMenu = transform.Find("Pause").gameObject;
        startMenu = transform.Find("Start").gameObject;
        hudMenu = transform.Find("HUD").gameObject;
        GameObject simObj = GameObject.Find("Simulation");
        sim = simObj.GetComponent<Simulation>();
    }

    public void SetActive(bool overrideActive) {
        active = overrideActive;

        if (!sim.running) {
            active = false;
        }

        sim.paused = active;
        pauseMenu.SetActive(active);
    }

    public void StopSimulation() {
        sim.End();

        active = false;
        pauseMenu.SetActive(active);
        hudMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            if (!keyDown) {
                keyDown = true;

                SetActive(!active);
            }
        } else {
            keyDown = false;
        }
    }
}