using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    private Simulation sim;
    private GameObject hudMenu;
    private GameObject bar;
    private GameObject inspectMenu;

    private bool keyDown = false;
    private bool active = true;
    private double increaseSpeed = 1e-30;
    private double increaseDirection = 0;

    private void Awake() {
        hudMenu = transform.Find("HUD").gameObject;
        bar = hudMenu.transform.Find("Bar").gameObject;
        inspectMenu = hudMenu.transform.Find("Inspect").gameObject;
        GameObject simObj = GameObject.Find("Simulation");
        sim = simObj.GetComponent<Simulation>();
    }

    public void SetActive(bool overrideActive) {
        active = overrideActive;

        if (!sim.running) {
            active = false;
        }

        hudMenu.SetActive(active);
    }

    public void IncreaseTimeSpeed(bool start) {
        if (!start) {
            increaseDirection = 0;
        } else {
            increaseDirection = 1;
        }
    }

    public void DecreaseTimeSpeed(bool start) {
        if (!start) {
            increaseDirection = 0;
        } else {
            increaseDirection = -1;
        }
    }

    public void Inspect(Dictionary<string, string> info) {
        inspectMenu.SetActive(true);

        string formattedInfo = "";
        foreach(KeyValuePair<string, string> entry in info) {
            formattedInfo += "\n" + entry.Key + ": " + entry.Value;
        }

        Text infoText = inspectMenu.transform.Find("Info").gameObject.GetComponent<Text>();
        infoText.text = formattedInfo.Trim();
    }

    void Update() {
        if (Input.GetKey(KeyCode.H)) {
            if (!keyDown) {
                keyDown = true;

                SetActive(!active);
            }
        } else {
            keyDown = false;
        }

        double seconds = sim.simTime * Constants.solarScale;
        TimeSpan span = TimeSpan.FromSeconds(seconds);
        DateTime dateTime = new DateTime();
        dateTime = dateTime.AddDays(span.Days);

        Text time = bar.transform.Find("Time").gameObject.GetComponent<Text>();
        time.text = string.Format("{0:D2}:{1:D2}:{2:D2}", span.Hours, span.Minutes, span.Seconds);

        Text date = bar.transform.Find("Date").gameObject.GetComponent<Text>();
        date.text = string.Format("{0:D2}/{1:D2}/{2:D4}", dateTime.Day, dateTime.Month, dateTime.Year);

        double timeSpeed = sim.timeSpeed * Constants.solarScale;
        double minuteSpeed = timeSpeed / 60;
        double hourSpeed = timeSpeed / (60 * 60);
        double daySpeed = timeSpeed / (24 * 60 * 60);
        double weekSpeed = timeSpeed / (7 * 24 * 60 * 60);
        double monthSpeed = timeSpeed / (30 * 24 * 60 * 60);
        double yearSpeed = timeSpeed / (365 * 24 * 60 * 60);

        Text timeSpeedText = bar.transform.Find("TimeSpeed").gameObject.GetComponent<Text>();
        if (yearSpeed > 1) {
            timeSpeedText.text = string.Format("{0:F2} years/sec", yearSpeed);
        } else if (monthSpeed > 1) {
            timeSpeedText.text = string.Format("{0:F2} months/sec", monthSpeed);
        } else if (weekSpeed > 1) {
            timeSpeedText.text = string.Format("{0:F2} weeks/sec", weekSpeed);
        } else if (daySpeed > 1) {
            timeSpeedText.text = string.Format("{0:F2} days/sec", daySpeed);
        } else if (hourSpeed > 1) {
            timeSpeedText.text = string.Format("{0:F2} hours/sec", hourSpeed);
        } else if (minuteSpeed > 1) {
            timeSpeedText.text = string.Format("{0:F2} mins/sec", minuteSpeed);
        } else {
            timeSpeedText.text = string.Format("{0:F2} secs/sec", timeSpeed);
        }

        increaseSpeed = Math.Max(100, Math.Abs(increaseSpeed) * 1.00000001) * increaseDirection;
        sim.timeSpeed = Mathf.Max(0, sim.timeSpeed + (float)increaseSpeed);
    }
}