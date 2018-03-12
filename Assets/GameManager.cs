using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public Character bossToFight;

    public static GameManager instance;

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
