using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mirror
{
    public class Connect : NetworkBehaviour
    {
        [SerializeField] private Text IPaddress;
        [SerializeField] private GameObject IPaddressLabel;
        [SerializeField] private GameObject panel;
        [SerializeField] private UnityEngine.UI.Button discButton;
        NetworkManager manager;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        public void StartHost()
        {
            if (!NetworkClient.active)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    discButton.GetComponentInChildren<Text>().text = "Stop Host";
                    GetLocalIPv4();
                    manager.StartHost();
                    Connected();
                    Status();
                }
            }
        }

        public void StartClient()
        {
            if (!NetworkClient.active)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    discButton.GetComponentInChildren<Text>().text = "Disconnect";
                    manager.StartClient();
                    Connected();
                    Status();
                }
                manager.networkAddress = IPaddress.text.ToString();
            }
        }

        public void Disconnect()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                manager.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                manager.StopClient();
            }
            else if (NetworkServer.active)
            {
                manager.StopServer();
            }
        }

        public string GetLocalIPv4()
        {
            print(Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString());
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }

        public void Status()
        {
            IPaddressLabel.GetComponent<Text>().text = manager.networkAddress;
        }

        public void Connected()
        {
            panel.SetActive(false);
            IPaddressLabel.SetActive(true);
            discButton.gameObject.SetActive(true);
        }
    }
}

