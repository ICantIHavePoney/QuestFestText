using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class MenuManager : MonoBehaviour {

    [SerializeField]
    private InputField IPInput;

    public static MenuManager instance;

    [SerializeField]
    private Text IPErrorText;

    private bool canConnect;

    private bool isConnected;

	// Use this for initialization
	void Start () {
		if(instance != this)
        {
            Destroy(instance);
        }
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void OnIpChanged()
    {
        if (Regex.IsMatch(IPInput.text, "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}$")) {
            NetworkManager.instance.SetAdressInput(IPInput.text);
            IPErrorText.text = "Adresse IP Valide";
            IPErrorText.color = Color.green;
            canConnect = true;
        }
        else
        {
            IPErrorText.text = "Adresse IP non Valide";
            IPErrorText.color = Color.red;
            canConnect = false;
        }
    }

    public void OnHostButtonClick()
    {
        NetworkManager.instance.isHost = true;

        NetworkManager.instance.Init();
    }

    public void OnConnectButtonClick()
    {
        NetworkManager.instance.isHost = false;

        NetworkManager.instance.Init();  

    }

    public void ConnectSuccess()
    {
        Debug.Log("C'est bon ! on charge le perso now !");
    }

    public void ConnectFailed()
    {
        Debug.Log("Hé merde, on est pas co");
    }
}
