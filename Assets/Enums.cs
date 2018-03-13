using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType{

	connection,
	disconnection,
    beginCombat,
	attack,
    newBoss,
    answer,

}

public enum AttackType
{
    Contact,
    Magical,
    Ranged
}
public enum CharacterType
{
    Player,
    Enemy
}

