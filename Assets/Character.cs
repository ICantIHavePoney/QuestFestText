﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

[Serializable]
public class Character  {

    private string name;

    public CharacterType type;

    public float strenght { get; set; }

    public float agility { get; set; }

    public float intelligence { get; set; }

    public int health { get; set; }

    public int id { get; set; }

    public int maxHealth { get; set; }

    public Character(string n, CharacterType t, int s, int a, int i, int h)
    {
        name = n;

        type = t;

        strenght = s;

        agility = a;

        intelligence = i;

        health = h;

        maxHealth = h;
    }

    public string GetName()
    {
        return name;
    }

}
