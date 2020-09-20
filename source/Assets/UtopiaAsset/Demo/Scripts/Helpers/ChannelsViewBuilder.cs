using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ChannelsViewBuilder : MonoBehaviour
{
    public Transform ChannelsGUIPrefab;
	public Vector3 PrefabRotationEauler;
	public float placeTimerTimeout = 0.05f;
	public UtopiaConnectionExampleExtended ConnectionController;

	protected JArray channelsData;
	protected List<Transform> channelObjects;
	protected int placeIndex = 0;
	protected HexGridBuilder HexGBuilder;
	
	protected float hx = 0;
	protected float hy = 0;
	protected float prefab_posY = 0.01f;
	protected string channels_group_name = "ChannelsGroup";
	protected GameObject channels_group_obj;
	
	public void Start() {
		HexGBuilder = this.GetComponent<HexGridBuilder>();
	}
	
	public void buildChannels(JArray channels_Data) {
		this.channelsData = channels_Data;
		placeIndex = 0;
		//foreach(JObject channelJData in channelsData) {
		//	Debug.Log(channelJData);
		//}
		
		channels_group_obj = new GameObject(channels_group_name);
		
		StartCoroutine(PlaceGUIElementCoroutine());
	}

	IEnumerator PlaceGUIElementCoroutine()
	{
		float scale_grid_x = 1.25f;
		float scale_grid_y = 1.45f;

		while(placeIndex < channelsData.Count)
		{
			Vector2 prefab_position = HexGBuilder.getHexPosition(placeIndex, hx, hy);
			hx = prefab_position.x * scale_grid_x;
			hy = prefab_position.y * scale_grid_y;
			
			//instantiate
			Transform channelElementTransform = InstantiateChannelGUIElement(new Vector3(hx, prefab_posY, hy));
			//set object name
			channelElementTransform.gameObject.name = "channel" + placeIndex.ToString();
			//set object parent
			channelElementTransform.SetParent(channels_group_obj.transform);
			//set channel image
			ContactController controller = channelElementTransform.gameObject.GetComponent<ContactController>();
			string channelID = channelsData[placeIndex]["channelid"].ToString();
			controller.changeAvatar(ConnectionController.getChannelAvatar(channelID));

			placeIndex++;
			yield return new WaitForSeconds(placeTimerTimeout);
		}
	}
	
	public void clearChannels() {
		hx = 0;
		hy = 0;
		placeIndex = 0;
		
		GameObject channels_group_obj = GameObject.Find(channels_group_name);
		if(channels_group_obj != null) {
			//objects has been added to the scene
			Destroy(channels_group_obj);
		}
	}
	
	Transform InstantiateChannelGUIElement(Vector3 elementPosition) {
		return Instantiate(
			ChannelsGUIPrefab,
			elementPosition,
			Quaternion.Euler(PrefabRotationEauler)
		);
	}
}
