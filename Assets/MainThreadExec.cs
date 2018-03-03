using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadExec : MonoBehaviour {

    public readonly static Queue<Action> stuffToExecute = new Queue<Action>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        while(stuffToExecute.Count > 0)
        {
            stuffToExecute.Dequeue().Invoke();
        }

	}
}
