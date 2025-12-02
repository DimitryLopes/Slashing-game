using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionScreen : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField ipAddressInputField;
    [SerializeField]
    private Button connectButton;

    private void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    private void OnConnectButtonClicked()
    {
        string ipAddress = ipAddressInputField.text;
        Debug.Log("Attempting to connect to IP Address: " + ipAddress);
        // Add your connection logic here
    }
}
