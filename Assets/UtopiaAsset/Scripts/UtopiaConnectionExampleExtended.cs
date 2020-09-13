using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtopiaLib;

public class UtopiaConnectionExampleExtended : MonoBehaviour
{
	public string protocol = "http";
    //public string api_version = "1.0";

    public InputField inputHost;
    public InputField inputToken;
    public InputField inputPort;
    public Text responseTextObj;

	public GameObject PanelConnection;
	public GameObject PanelPlayground;

	protected UtopiaLib.Client client;
	protected string endpoint;
    protected string api_host;

	public void Start() {
		//DontDestroyOnLoad(this);
	}
	
    bool connect(string host = "127.0.0.1", string token = "", int port = 20000) {
        //endpoint = protocol + "://" + host + port.ToString() + "/api/" + api_version;
        api_host = protocol + "://" + host;

        client = new UtopiaLib.Client(api_host, port, token);
        //client.api_version = api_version;
        return client.checkClientConnection();
	}

	public void guiActionConnect() {
        string host = inputHost.text;
        string token = inputToken.text;
        int port = 20000;
        Int32.TryParse(inputPort.text, out port);
		
		bool is_connected = false;
		string response = "";
		try {
			is_connected = connect(host, token, port);
			response = client.getSystemInfo().ToString();
		} catch (System.Net.Sockets.SocketException ex) {
			response = "SocketException: " + ex.Message;
		} catch (Exception ex) {
			response = "Exception: " + ex.Message;
		}
		responseTextObj.text = response;
		Debug.Log(response);
		
		PanelConnection.SetActive(!is_connected);
		PanelPlayground.SetActive(is_connected);
    }
}
