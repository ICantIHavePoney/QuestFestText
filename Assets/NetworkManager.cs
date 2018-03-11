using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

	private UdpClient client;

    public static NetworkManager instance;

	private int serverPort = 60;

    private int clientPort = 6060;

	private IPEndPoint addressToConnect;

    private string inputAddress;

	public bool isHost;

	private bool gameLaunched = false;


    public int roomSize = 3;

    public Character newChara;

    private Dictionary<IPEndPoint, Character> connectedClients;

    private float serverLastMessage;

    public List<Character> otherCharacters;

    private AsyncCallback callback;

    private bool isConnected = false;

	void Awake()
	{
		connectedClients = new Dictionary<IPEndPoint, Character>();
        otherCharacters = new List<Character>();

        callback = new AsyncCallback(ReceiveCallback);
    }

	// Use this for initialization
	void Start () {
        if(instance != this)
        {
            Destroy(instance);
        }


        instance = this;
		Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Init(){

        if (isHost)
        {
            client = new UdpClient(serverPort);
        }
        else
        {
            client = new UdpClient(clientPort);
            addressToConnect = new IPEndPoint(IPAddress.Parse(inputAddress), serverPort);

            newChara = new Character("Nayos", CharacterType.Player, UnityEngine.Random.Range(0, 11), UnityEngine.Random.Range(0, 11), UnityEngine.Random.Range(0, 11), 100);

            byte[] message = GetMessage(MessageType.connection, newChara.ToBytesArray());

            client.Send(message, message.Length, addressToConnect);

        }
        client.BeginReceive(callback, null);        
	}

	public void ReceiveCallback(IAsyncResult ar)
	{
        try
        {
            IPEndPoint senderInfo = new IPEndPoint(0, 0);

            byte[] message = client.EndReceive(ar, ref senderInfo);

            MessageType type = ParseMessageType(message);

            message = message.Slice(4);

            switch (type)
            {
                case MessageType.connection:
                        Character newCharacter = (Character)message.ToObject();
                        if (isHost)
                        {
                            if(!gameLaunched && connectedClients.Count < roomSize)
                            message = GetMessage(MessageType.connection, newCharacter.ToBytesArray());

                            if (!connectedClients.ContainsKey(senderInfo))
                            {
                                connectedClients.Add(senderInfo, newCharacter);
                            }

                            ServerToOthers(message, senderInfo);

                            message = GetMessage(MessageType.connection, new byte[0]);

                            client.Send(message, message.Length, senderInfo);    

                            Debug.Log(newCharacter.GetName() + "S'est connecté");
                            
                        }
                        else
                        {
                            if (newCharacter != null)
                            {
                                otherCharacters.Add(newCharacter);
                            MenuManager.instance.DisplayConnectedChars(newCharacter);
                            }
                            else
                            {
                                MenuManager.instance.ConnectSuccess();
                                Debug.Log("On est bien connecté !");

                            }
                        }
                    break;
                case MessageType.disconnection:
                    Character disconnectedOne = (Character)message.ToObject();
                    if (isHost)
                    {
                        message = GetMessage(MessageType.disconnection, disconnectedOne.ToBytesArray());

                        ServerToOthers(message, senderInfo);
                        connectedClients.Remove(senderInfo);

                        Debug.Log(disconnectedOne.GetName() + "S'est déconnecté");
                    }

                    else
                    {
                        Debug.Log("Vous avez été déconnecté");
                        otherCharacters.Remove(disconnectedOne);
                    }
                    break;
            }

            client.BeginReceive(callback, null);
        }
        catch (ObjectDisposedException)
        {
            Debug.Log("Connection closed");
        }
        catch(Exception err)
        {
            Debug.Log(err);
        }
		
	}

    private IEnumerator AfkDisconnect(IPEndPoint clientIP)
    {
        yield return new WaitForSeconds(10);
        Character disconnectedChar;
        connectedClients.TryGetValue(clientIP, out disconnectedChar);

        byte[] message = GetMessage(MessageType.disconnection, disconnectedChar.ToBytesArray());

        client.Send(message, message.Length, clientIP);

        Debug.Log(disconnectedChar.GetName() + " à été déconnecté");

        connectedClients.Remove(clientIP);

    }



    public byte[] GetMessage(MessageType type, byte[] contentArray)
    {
        byte[] message = new byte[4 + contentArray.Length];

        byte[] messageTypeArray = BitConverter.GetBytes((int)type);

        messageTypeArray.CopyTo(message, 0);

        contentArray.CopyTo(message, 4);

        return message;
    }

    private MessageType ParseMessageType(byte[] message)
    {
        byte[] messageTypeArray = new byte[4];

        Array.Copy(message, 0, messageTypeArray, 0, messageTypeArray.Length);

        MessageType type = (MessageType)BitConverter.ToInt32(messageTypeArray, 0);

        return type;
    }

    private IEnumerator ConnectCheck()
    {
        yield return new WaitForSeconds(10);
        isConnected = false;
        addressToConnect = new IPEndPoint(0, 0);
        Debug.Log("L'adresse n'est pas bonne ou le serveur n'est pas en ligne");
        MenuManager.instance.ConnectFailed();

    }

    private void ServerToOthers(byte[] message, IPEndPoint clientToExclude){



        foreach(var clientItem in connectedClients.Keys)
        {
            if(clientItem != clientToExclude)
            {
                client.Send(message, message.Length, clientItem);
            }
        }
	}

    private void ServerToAll(byte[] message)
    {
        foreach(var clientItem in connectedClients.Keys)
        {
            client.Send(message, message.Length, clientItem);
        }
    }

    public void SetAdressInput(string ip)
    {
        inputAddress = ip;
    }
}