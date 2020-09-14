using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactController : MonoBehaviour
{
    public RawImage avatar;
	
	public void changeAvatar(Texture2D newTexture) {
		avatar.texture = newTexture;
	}
}
