using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterPanel : MonoBehaviour {

    public Character character;

    public Text characterName;

    public Text strenghtText;

    public Text agilityText;

    public Text intelligenceText;

    public Text healthText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void DisplayCharacter()
    {
        characterName.text = character.GetName();

        strenghtText.text = character.strenght.ToString();

        agilityText.text = character.agility.ToString();

        intelligenceText.text = character.intelligence.ToString();

        healthText.text = character.health.ToString();

    }
}
