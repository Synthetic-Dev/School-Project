using UnityEngine;

public class Controls : MonoBehaviour {
    private bool active = false;

    public void Click() {
        active = !active;
        gameObject.SetActive(active);
    }
}