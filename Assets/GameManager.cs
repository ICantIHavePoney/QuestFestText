using System.Collections;
using System.Collections.Generic;
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
            
        }
        if (canAttack)
        {
            NetworkManager.instance.SendToServer(MessageType.attack);
        }
    }


    public void Magic()
    {

    }

    public void Throw()
    {

    }

    public void UpdateCharacters(MessageType attackType)
    {

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
            bossToFight = new Character("Ivan the terrible !", CharacterType.Enemy, UnityEngine.Random.Range(8, 12), UnityEngine.Random.Range(8, 12), UnityEngine.Random.Range(8, 12), 400);
        }
    }

}
