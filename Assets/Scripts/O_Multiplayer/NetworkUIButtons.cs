using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkUIButtons : MonoBehaviour {

    [SerializeField] private GameObject buttonUI;

    private void Awake() {
        buttonUI.SetActive(true);
    }
    public void StartHost() {
        Debug.Log("Starting Host...");
        NetworkManager.Singleton.StartHost();
        buttonUI.SetActive(false);
    }

    public void StartClient() {
        Debug.Log("Starting Client...");
        NetworkManager.Singleton.StartClient();
        buttonUI.SetActive(false);
    }

    public void StartServer() {
        Debug.Log("Starting Server...");
        NetworkManager.Singleton.StartServer();
        buttonUI.SetActive(false);
    }
}
