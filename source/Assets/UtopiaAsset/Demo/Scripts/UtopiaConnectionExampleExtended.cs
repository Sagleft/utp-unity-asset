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
	public bool isDebug = false;
    //public string api_version = "1.0";

    public InputField inputHost;
    public InputField inputToken;
    public InputField inputPort;
    public Text responseTextObj;

	public GameObject PanelConnection;
	public GameObject PanelPlayground;
	public RawImage userAvatarImage;
	public Text BalanceText;
	public GameObject WalletCanvasObject;
	
	public Text UserNameText;
	public Text UserMoodText;
	
	public Vector3[] camPositions;
	public Vector3[] camRotations;
	public float cameraTransformSpeed = 1.0f;
	
	public Transform ContactPrefab;

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
	protected HexGridBuilder HexGBuilder;
	protected ChannelsViewBuilder ChannelsBuilder;
	
	protected float accountBalance = 0.0f;

	public void Start() {
		//DontDestroyOnLoad(this);
		//find UI object with wallet canvas
		//WalletCanvasObject = GameObject.Find("WalletCanvas");
		ChannelsBuilder = this.GetComponent<ChannelsViewBuilder>();
		HexGBuilder     = this.GetComponent<HexGridBuilder>();
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
		if(isDebug) {
			Debug.Log(response);
		}
		
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
	
	void updateAccountBalance() {
		//TODO: checkClientConnection
		accountBalance = (float) client.getBalance();
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
		//get balance
		updateAccountBalance();
	}
	
	public Texture2D getAccountAvatar(string account_pubkey) {
		string avatar_base64 = client.getContactAvatar (account_pubkey);
		byte[] avatar_bytes = System.Convert.FromBase64String(avatar_base64);
		Texture2D avatar_texture = new Texture2D(1, 1);
		avatar_texture.LoadImage( avatar_bytes );
		return avatar_texture;
	}

	public Texture2D getChannelAvatar(string channelID) {
		string image_base64 = client.getChannelAvatar (channelID);
		byte[] image_bytes = System.Convert.FromBase64String(image_base64);
		Texture2D texture = new Texture2D(1, 1);
		texture.LoadImage( image_bytes );
		return texture;
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
		clearPlayground();
		moveCamByPointIndex(0);
		loadContacts();
	}
	
	void loadChannelsData() {
		//Type: 0-All, 1-Recent, 2-My, 3-Friends, 4-Bookmarked, 5-Joined, 6-Opened
		string channel_type = "0"; //joined channels
		string search_filter = "";
		UtopiaLib.QueryFilter filter = new UtopiaLib.QueryFilter();
		//limit channels number in response
		filter.limit = "33";
		
		JArray channelsData = client.getChannels(
			channel_type,
			search_filter,
			filter
		);
		ChannelsBuilder.buildChannels(channelsData);
		if(isDebug) {
			Debug.Log(channelsData);
		}
	}

	public void guiActionShowChannels() {
		clearPlayground();
		moveCamByPointIndex(1);
		loadChannelsData();
	}

	public void guiActionShowWallet() {
		clearPlayground();
		WalletCanvasObject.SetActive(true);
		moveCamByPointIndex(2);
		loadWalletData();
	}

	void loadWalletData() {
		updateAccountBalance();
		BalanceText.text = "Balance: " + accountBalance.ToString() + " CRP";
	}
	
	protected string contacts_group_name = "ContactsGroup";
	
	
	void clearPlayground() {
		//find GameObject contacts group
		GameObject contacts_group_obj = GameObject.Find(contacts_group_name);
		if(contacts_group_obj != null) {
			//objects has been added to the scene
			Destroy(contacts_group_obj);
		}
		ChannelsBuilder.clearChannels();
		WalletCanvasObject.SetActive(false);
	}
	
	void loadContacts() {
		GameObject contacts_group_obj = new GameObject(contacts_group_name);
		
		JArray contacts_arr = client.getContacts();
		int i = 0;
		float prefab_posY = 0.01f;
		
		float hx = 0; //reference start position, x
		float hy = 0; //y
		
		//parameters for stretching the mesh
		float scale_grid_x = 1.25f;
		float scale_grid_y = 1.45f;
		
		Quaternion prefab_rotation = Quaternion.Euler(90, 0, 0);
		foreach (JObject contact_obj in contacts_arr) {
			//Debug.Log(contact_obj["nick"].ToString());
			Vector2 prefab_position = HexGBuilder.getHexPosition(i, hx, hy);
			//Debug.Log(prefab_position);
			hx = prefab_position.x * scale_grid_x;
			hy = prefab_position.y * scale_grid_y;
			
			//adding a prefab to the scene
			Transform contact_transform = Instantiate(
				ContactPrefab,
				new Vector3(hx, prefab_posY, hy),
				prefab_rotation
			);
			contact_transform.gameObject.name = "contact" + i.ToString();
			
			//set the parent object
			contact_transform.SetParent(contacts_group_obj.transform);
			
			//add an avatar
			ContactController controller = contact_transform.gameObject.GetComponent<ContactController>();
			string contact_pubkey = contact_obj["pk"].ToString();
			controller.changeAvatar(getAccountAvatar(contact_pubkey));
			
			i++;
		}
	}
}
