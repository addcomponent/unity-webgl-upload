using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour 
{
	[SerializeField]
	private WebGLBridge _webGLBridge;

	[SerializeField]
	private InputField _inputField;

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Text _labelLog;

	[SerializeField]
	private DeviceCameraController _deviceCameraController;


	void Start()
	{
		_webGLBridge._onLogMessage = Log;
	}

	public void SelectImage()
	{
		_webGLBridge.SelectImage (GetMaxSize (), (bool success, Texture2D texture) => 
		{
			if(texture != null)
			{
				SetImage (texture);
			}
		});
	}

	public void EnableCamera()
	{
		#if UNITY_EDITOR
		EnableUnityCamera ();
		#else
		_webGLBridge.EnableCamera (GetMaxSize (), (bool success, Texture2D texture) => 
		{
			if(!success)
			{
				EnableUnityCamera ();
			}
			else if(texture != null)
			{
				SetImage (texture);	
			}
		});
		#endif
	}

	private void EnableUnityCamera()
	{
		_deviceCameraController.ShowCamera (((Texture2D texture) => 
		{
			if(texture != null)
			{
				SetImage (texture);
			}
		}));
	}


	private void SetImage(Texture2D texture)
	{
		_image.GetComponent <AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;
		_image.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero, 1);

		Resources.UnloadUnusedAssets ();

		Log("Picture Loaded " + texture.width + "x" + texture.height + "px\n");
	}

	private int GetMaxSize()
	{
		int maxSize = -1;

		if(!string.IsNullOrEmpty (_inputField.text))
		{
			int.TryParse (_inputField.text, out maxSize);	
		}

		Log ((maxSize == -1 ? "No Resize" : "Max size " + maxSize));

		return maxSize;
	}



	public void OpenWebsite()
	{
		Application.OpenURL ("http://addcomponent.com");
	}

	public void Log(string message)
	{
		_labelLog.text += "\n" + message;
	}
}