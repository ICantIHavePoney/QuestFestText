using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public Character bossToFight;

    private int target;

    private bool canAttack;

    public static GameManager instance;

    public GameObject characterDisplay;

    public Character character;

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


    public void Attack()
    {
        if (NetworkManager.instance.isHost)
        {
            int targetID = UnityEngine.Random.Range(1, 4);

            Character target = null;

            IPEndPoint key = null;

            foreach(var item in NetworkManager.instance.connectedClients)
            {
                if(item.Value.id == targetID)
                {
                    target = item.Value;
                    key = item.Key;
                    break;
                }
            }

            target.health -= DamagesCalculations(character, AttackType.Contact);
            NetworkManager.instance.connectedClients[key] = target;

            byte[] message = NetworkManager.instance.GetMessage(MessageType.attack, target.ToBytesArray());

            NetworkManager.instance.ServerToAll(message);
        }
        else
        {
            NetworkManager.instance.SendToServer(MessageType.attack, BitConverter.GetBytes((int)AttackType.Contact));
        }
    }


    public void Magic()
    {
        if (NetworkManager.instance.isHost)
        {
            int targetID = UnityEngine.Random.Range(1, 4);

            Character target = null;

            IPEndPoint key = null;

            foreach (var item in NetworkManager.instance.connectedClients)
            {
                if (item.Value.id == targetID)
                {
                    target = item.Value;
                    key = item.Key;
                    break;
                }
            }

            target.health -= DamagesCalculations(character, AttackType.Magical);
            NetworkManager.instance.connectedClients[key] = target;

            byte[] message = NetworkManager.instance.GetMessage(MessageType.attack, target.ToBytesArray());

            NetworkManager.instance.ServerToAll(message);
        }
        else
        {
            NetworkManager.instance.SendToServer(MessageType.attack, BitConverter.GetBytes((int)AttackType.Magical));
        }
    }

    public void Throw()
    {
        if (NetworkManager.instance.isHost)
        {
            int targetID = UnityEngine.Random.Range(1, 4);

            Character target = null;

            IPEndPoint key = null;

            foreach (var item in NetworkManager.instance.connectedClients)
            {
                if (item.Value.id == targetID)
                {
                    target = item.Value;
                    key = item.Key;
                    break;
                }
            }

            target.health -= DamagesCalculations(character, AttackType.Ranged);
            NetworkManager.instance.connectedClients[key] = target;

            byte[] message = NetworkManager.instance.GetMessage(MessageType.attack, target.ToBytesArray());

            NetworkManager.instance.ServerToAll(message);
        }
        else
        {
            NetworkManager.instance.SendToServer(MessageType.attack, BitConverter.GetBytes((int)AttackType.Ranged));
        }
    }


    public int DamagesCalculations(Character attacker, AttackType type)
    {
        int damages = 0;

        switch (type)
        {
            case AttackType.Contact:

                damages = (int)UnityEngine.Random.Range(attacker.strenght * 0.8f, attacker.strenght * 1.5f);
                break;

            case AttackType.Magical:
                damages = (int)UnityEngine.Random.Range(attacker.intelligence * 0.8f, attacker.intelligence * 1.5f);
                break;

            case AttackType.Ranged:
                damages = (int)UnityEngine.Random.Range(attacker.agility * 0.8f, attacker.agility * 1.5f);
                break;
        }

        return damages;
    }

    public IEnumerator reloadTime()
    {

        yield return new WaitForSeconds(10);

        canAttack = true;
    }


    public void CreateCharacter()
    {
        if (!NetworkManager.instance.isHost)
        {
            character = new Character("Toto", CharacterType.Player, UnityEngine.Random.Range(3, 11), UnityEngine.Random.Range(3, 11), UnityEngine.Random.Range(3, 11), 100);
        }

        else
        {
            character = new Character("Ivan the terrible !", CharacterType.Enemy, UnityEngine.Random.Range(8, 12), UnityEngine.Random.Range(8, 12), UnityEngine.Random.Range(8, 12), 400);
        }
    }

}
