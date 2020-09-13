using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtopiaLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
	public RawImage userAvatarImage;
	
	public Text UserNameText;
	public Text UserMoodText;

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
		loadAccountData();
    }
	
	public JObject getClientContact() {
		return client.getOwnContact ();
	}
	
	public string getClientPubkey() {
		JObject account_data = client.getOwnContact ();
		//Debug.Log(account_data.ToString());
		return account_data ["pk"].ToString ();
	}
	
	public void loadAccountData() {
		//get client pubkey
		string user_pubkey = getClientPubkey();
		//load avatar
		userAvatarImage.texture = getAccountAvatar(user_pubkey);
		//load username
		JObject contact_data = getClientContact();
		
		UserNameText.text = contact_data ["nick"].ToString ();
		UserMoodText.text = contact_data ["moodMessage"].ToString ();
	}
	
	public Texture2D getAccountAvatar(string account_pubkey) {
		string avatar_base64 = client.getContactAvatar (account_pubkey);
		byte[] avatar_bytes = System.Convert.FromBase64String(avatar_base64);
		Texture2D avatar_texture = new Texture2D(1, 1);
		avatar_texture.LoadImage( avatar_bytes );
		return avatar_texture;
	}
}
