using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestMultiplayer : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    void Awake()
    {
        m_NetworkManager = FindObjectOfType<NetworkManager>();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 600, 600));
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        var networkManager = FindObjectOfType<NetworkManager>();
        if (GUILayout.Button("Host")) networkManager.StartHost();
        if (GUILayout.Button("Client")) networkManager.StartClient();
        if (GUILayout.Button("Server")) networkManager.StartServer();
    }

    static void StatusLabels()
    {
        var networkManager = FindObjectOfType<NetworkManager>();
        var mode = networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        networkManager.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}