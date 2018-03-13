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

    public bool gameLaunched = false;

    private int id;

    public int roomSize;

    public Dictionary<IPEndPoint, Character> connectedClients;

    private Character currentPlayer;

    private float serverLastMessage;

    public List<Character> otherCharacters;

    private AsyncCallback callback;
    
    private Coroutine answerCheck;

    private int failCount = 0;

    void Awake()
    {
        connectedClients = new Dictionary<IPEndPoint, Character>();
        otherCharacters = new List<Character>();

        callback = new AsyncCallback(ReceiveCallback);
    }

    // Use this for initialization
    void Start() {
        if (instance != this)
        {
            Destroy(instance);
        }

        roomSize = 3;


        id = 1;
        instance = this;
        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update() {
    }

    public void Init() {

        if (isHost)
        {
            client = new UdpClient(serverPort);

        }
        else
        {
            client = new UdpClient(clientPort);
            addressToConnect = new IPEndPoint(IPAddress.Parse(inputAddress), serverPort);

            byte[] message = GetMessage(MessageType.connection, GameManager.instance.character.ToBytesArray());


            client.Send(message, message.Length, addressToConnect);

            answerCheck = StartCoroutine(AnswerCheck());

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
                        if (!gameLaunched && connectedClients.Count < roomSize) { }
                        

                            MainThreadExec.stuffToExecute.Enqueue(() => MenuManager.instance.DisplayConnectedChars(newCharacter));

                            newCharacter.id = id;

                            message = GetMessage(MessageType.connection, newCharacter.ToBytesArray());

                            if (!connectedClients.ContainsKey(senderInfo))
                            {
                                connectedClients.Add(senderInfo, newCharacter);
                            }

                            ServerToOthers(message, senderInfo);

                            message = GetMessage(MessageType.answer, BitConverter.GetBytes(id));

                            client.Send(message, message.Length, senderInfo);

                            id++;

                            message = GetMessage(MessageType.newBoss, GameManager.instance.bossToFight.ToBytesArray());

                            ServerToAll(message);

                            Debug.Log(newCharacter.GetName() + " S'est connecté");
                        
                    }
                    else
                    {
                        if (newCharacter != null)
                        {
                            Debug.Log(newCharacter);
                            otherCharacters.Add(newCharacter);
                            MainThreadExec.stuffToExecute.Enqueue(() => MenuManager.instance.DisplayConnectedChars(newCharacter));
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

                        Debug.Log(disconnectedOne.GetName() + " S'est déconnecté");
                    }

                    else
                    {
                        otherCharacters.Remove(disconnectedOne);
                    }
                    break;

                case MessageType.newBoss:
                    Character newBoss = (Character)message.ToObject();
                    if (!isHost)
                    {
                        MainThreadExec.stuffToExecute.Enqueue(() =>
                        {
                            GameManager.instance.bossToFight = newBoss;
                            MenuManager.instance.DisplayConnectedChars(newBoss);
                        });
                    }
                    break;

                case MessageType.beginCombat:
                    if (!isHost)
                    {
                        MainThreadExec.stuffToExecute.Enqueue(() =>
                        {
                            MenuManager.instance.GameMenuPanel.SetActive(true);
                        });
                    }
                    break;


                case MessageType.attack:
                    if (isHost)
                    {
                        Character attacker = connectedClients[senderInfo];

                        GameManager.instance.character.health -= GameManager.instance.DamagesCalculations(attacker, GetAttackType(message));

                        client.Send(BitConverter.GetBytes((int)MessageType.answer), BitConverter.GetBytes((int)MessageType.answer).Length, senderInfo);

                        ServerToAll(GetMessage(MessageType.attack, GameManager.instance.character.ToBytesArray()));
                    }
                    else
                    {
                        Character victim = (Character)message.ToObject();
                        if (victim.type == CharacterType.Enemy)
                        {
                            GameManager.instance.bossToFight = victim;
                        }
                        else
                        {
                            if (victim.id != GameManager.instance.character.id)
                            {                    
                                for (int i = 0; i < otherCharacters.Count; i++)
                                {
                                    if (victim.id == otherCharacters[i].id)
                                    {
                                        otherCharacters[i] = victim;
                                    }
                                }
                            }
                            else
                            {
                                GameManager.instance.character = victim;
                            }
                        }
                    }
                     break;
                case MessageType.answer:
                    id = BitConverter.ToInt32(message, 0);
                    if (!isHost)
                    {

                        if (id != 0)
                        {
                            GameManager.instance.character.id = id;
                            MenuManager.instance.ConnectSuccess();
                        }
                        MainThreadExec.stuffToExecute.Enqueue(() => StopCoroutine(answerCheck));
                    }

                    break;
            }

            client.BeginReceive(callback, null);
        }
        catch (ObjectDisposedException)
        {
            Debug.Log("Connection closed");
        }
        catch (Exception err)
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


    private AttackType GetAttackType(byte[] message)
    {
        byte[] attackTypeArray = new byte[4];

        Array.Copy(message, 0, attackTypeArray, 0, attackTypeArray.Length);

        return (AttackType)BitConverter.ToInt32(attackTypeArray, 0);
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

    public void SendToServer(MessageType type, byte[] content = null)
    {
        byte[] message = BitConverter.GetBytes((int)type);

        if (content != null)
        {
            content.CopyTo(message, message.Length);
        }

        answerCheck = StartCoroutine(AnswerCheck());

        client.Send(message, message.Length, addressToConnect);
    }


    private IEnumerator AnswerCheck()
    {
        yield return new WaitForSeconds(10);
        if(failCount < 3)
        {
            failCount++;
        }
        else
        {
            //DO Stuff
        }
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

    public void ServerToAll(byte[] message)
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