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


	void Start()
	{
		_webGLBridge._onLogMessage = Log;
	}

	public void LoadPicture()
	{
		int maxSize = -1;

		if(!string.IsNullOrEmpty (_inputField.text))
		{
			int.TryParse (_inputField.text, out maxSize);	
		}

		Log ((maxSize == -1 ? "No Resize" : "Max size " + maxSize));

		_webGLBridge.SelectImage (maxSize, (Texture2D texture) => 
		{
			if(texture != null)
			{
				_image.GetComponent <AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;
				_image.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero, 1);

				Resources.UnloadUnusedAssets ();

				Log("Picture Loaded " + texture.width + "x" + texture.height + "px\n");
			}
		});
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