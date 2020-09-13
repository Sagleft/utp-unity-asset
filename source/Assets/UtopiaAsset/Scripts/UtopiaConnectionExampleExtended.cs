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
	
	public Vector3[] camPositions;
	public Vector3[] camRotations;
	public float cameraTransformSpeed = 1.0f;

	protected UtopiaLib.Client client;
	protected string endpoint;
    protected string api_host;
	
	protected bool isCamMoving = false;
	//protected bool isGUISuspended = false;
	//protected Vector3 camStartPosition;
	protected Vector3 camDestinationPosition;
	//protected Quaternion camStartQuaternion;
	protected Quaternion camDestinationQuaternion;
	protected float camTransformStartTime = 0.0f;
	protected float camDestinationRangeParam = 0.1f;

	public void Start() {
		//DontDestroyOnLoad(this);
	}
	
	public void Update() {
		//move the camera if this condition is enabled
		if(isCamMoving) {
			float transform_param = cameraTransformSpeed * Time.deltaTime;
			
			//lerp position
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, camDestinationPosition, transform_param);
			
			//lerp rotation
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, camDestinationQuaternion, transform_param);
			//Debug.Log(transform_param);
			
			//Debug.Log(transform_param2);
			Vector3 distanceVector = camDestinationPosition - Camera.main.transform.position;
			if (distanceVector.sqrMagnitude < camDestinationRangeParam * camDestinationRangeParam) {
				isCamMoving = false;
			}
		}
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
	
	public void moveCam(Vector3 dest, Vector3 newRotation) {
		//initiates smooth camera movement to a new position
		//camStartPosition   = Camera.main.transform.position;
		//camStartQuaternion = Camera.main.transform.rotation;
		
		camDestinationPosition   = dest;
		camDestinationQuaternion = Quaternion.Euler(newRotation.x, newRotation.y, newRotation.z);
		camTransformStartTime = Time.time;
		isCamMoving = true;
		//isGUISuspended = true;
	}
	
	void moveCamByPointIndex(int point_index = 0) {
		moveCam(camPositions[point_index], camRotations[point_index]);
	}
	
	public void guiActionShowContacts() {
		moveCamByPointIndex(0);
	}
	
	public void guiActionShowChannels() {
		moveCamByPointIndex(1);
	}
	
	public void guiActionShowWallet() {
		moveCamByPointIndex(2);
	}
}
