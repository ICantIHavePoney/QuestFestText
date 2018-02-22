using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;
using UnityEngine.UI;

public struct UdpState{
	public IPEndPoint i;
	public UdpClient c;
}

public class NetworkManager : MonoBehaviour {

	private UdpClient client;

	private int port = 6060;

	private IPAddress addressToConnect;

	private string inputAddress;

	private bool isHost = false;

	private bool gameLaunched = false;

	private int roomSize;

	private Dictionary<IPEndPoint, string> connectedClients;

	void Awake()
	{
		connectedClients = new Dictionary<IPEndPoint, string>();
	}

	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Init(){
		client = new UdpClient(port);
		UdpState state = new UdpState();
		state.c = client;
		if(isHost){
			addressToConnect = IPAddress.Any;
		}
		else {
			addressToConnect = IPAddress.Parse(inputAddress);
		}
		state.i = new IPEndPoint(addressToConnect, port);
		state.c.BeginReceive(new AsyncCallback(ReceiveCallback), state);
	}


	public void ReceiveCallback(IAsyncResult ar)
	{
		try{
			UdpState state = (UdpState)(ar.AsyncState);
			UdpClient client = state.c;
			IPEndPoint clientInfo = state.i;
		}
		catch (ObjectDisposedException)
		{
			Debug.Log("Connexion closed");
		}
		catch (Exception err)
		{
			Debug.Log(err);
		}
	}


	public void SendToOthers(UdpState client){
		
	}
}
