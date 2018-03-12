using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class MenuManager : MonoBehaviour {

    [SerializeField]
    private InputField IPInput; 

    public static MenuManager instance;

    public GameObject MainMenuPanel;

    public GameObject startGame;

    public GameObject LobbyPanel;

    public GameObject GamePanel;

    public GameObject connectedCharsPanel;

    public GameObject enemyDisplayPanel;

    [SerializeField]
    private Text ErrorText;

    private int HeightOffset = 250;

    public GameObject charPanel;

    private bool canConnect;

    private bool isConnected;

    private Coroutine connectRoutineCheck;

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
            ErrorText.text = "Adresse IP Valide";
            ErrorText.color = Color.green;
            canConnect = true;
        }
        else
        {
            ErrorText.text = "Adresse IP non Valide";
            ErrorText.color = Color.red;
            canConnect = false;
        }
    }


    public void StartGame()
    {
        if (NetworkManager.instance.isHost)
        {
            NetworkManager.instance.gameLaunched = true;

            NetworkManager.instance.ServerToAll(BitConverter.GetBytes((int)MessageType.beginCombat)) ;
        }

        LobbyPanel.SetActive(false);
        GamePanel.SetActive(true);

    }

    public void OnHostButtonClick()
    {
        NetworkManager.instance.isHost = true;

        MainMenuPanel.SetActive(false);

        LobbyPanel.SetActive(true);

        startGame.SetActive(true);

        GameManager.instance.CreateCharacter();

        DisplayConnectedChars(GameManager.instance.bossToFight);

        NetworkManager.instance.Init();
    }

    public void OnConnectButtonClick()
    {
        NetworkManager.instance.isHost = false;

        GameManager.instance.CreateCharacter();

        NetworkManager.instance.Init();

        connectRoutineCheck = StartCoroutine(ConnectFailed());

    }

    public void DisplayConnectedChars(Character charToDisplay)
    {

        Transform parent; 
        var temp = Instantiate(charPanel, connectedCharsPanel.transform);

        if (charToDisplay.type == CharacterType.Player)
        {
            parent = connectedCharsPanel.transform;
            temp.transform.position = new Vector3(parent.position.x, parent.position.y + HeightOffset, parent.position.z);
            HeightOffset -= 125;
        }
        else
        {
            parent = enemyDisplayPanel.transform;
            temp.transform.position = new Vector3(parent.position.x, parent.position.y, parent.position.z);
        }
        var tempScript = temp.GetComponent<CharacterPanel>();
        tempScript.character = charToDisplay;
        tempScript.DisplayCharacter();
    }

    public void ConnectSuccess()
    {
        MainThreadExec.stuffToExecute.Enqueue(() =>
        {
            StopCoroutine(connectRoutineCheck);

            MainMenuPanel.SetActive(false);

            LobbyPanel.SetActive(true);



            DisplayConnectedChars(GameManager.instance.character);

            foreach (var item in NetworkManager.instance.otherCharacters)
            {
                DisplayConnectedChars(item);
            }

            Debug.Log("C'est bon ! on charge le perso now !");
        });
    }

    public IEnumerator ConnectFailed()
    {
        yield return new WaitForSeconds(10);
        Debug.Log("Hé merde, on est pas co");
    }
}
