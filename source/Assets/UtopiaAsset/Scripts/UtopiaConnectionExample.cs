using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtopiaLib;

public class UtopiaConnectionExample : MonoBehaviour
{
	public string protocol = "http";
    //public string api_version = "1.0";

    public InputField inputHost;
    public InputField inputToken;
    public InputField inputPort;
    public Text responseTextObj;
	
	protected UtopiaLib.Client client;
	protected string endpoint;
    protected string api_host;
	
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
        connect(host, token, port);

        responseTextObj.text = client.getSystemInfo().ToString();
    }
}
