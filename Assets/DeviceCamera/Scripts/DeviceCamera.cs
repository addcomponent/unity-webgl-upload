//
//  Webcam.cs
//
//  Author:
//       Tomaz Saraiva <tomaz.saraiva@gmail.com>
//
//  Copyright (c) 2017 Tomaz Saraiva
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
	public delegate void OnCameraEnabled(bool success, string message);
	public delegate void OnCameraImage(Texture2D texture);

	private OnCameraEnabled _callbackEnabled;

	private RawImage _image;
	private RectTransform _imageRectTransform;
	private AspectRatioFitter _aspectRatioFitter;

	private WebCamTexture _webcamTexture;

	private int _cameraIndex = -1;
	private bool _enabled = false;
	private bool _ready = false;


	public void Enable(OnCameraEnabled callback, int cameraIndex = 0)
	{
		if(_image == null)
		{
			_image = GetComponentInChildren <RawImage> ();
			_imageRectTransform = _image.GetComponent <RectTransform> ();
		}

		if(_aspectRatioFitter == null)
		{
			_aspectRatioFitter = GetComponentInChildren <AspectRatioFitter> ();	
		}
			
		_callbackEnabled = callback;
		_cameraIndex = cameraIndex;
		_ready = false;
		_enabled = true;

		gameObject.SetActive (true);
	}

	public void Disable()
	{
		_enabled = false;
		_cameraIndex = -1;

		_image.texture = null;

		if(_webcamTexture != null)
		{
			_webcamTexture.Stop ();
			_webcamTexture = null;
		}

		if(_callbackEnabled != null)
		{
			_callbackEnabled = null;
		}

		gameObject.SetActive (false);
	}


	public void Pause()
	{
		if(_webcamTexture != null && _webcamTexture.isPlaying)
		{
			_webcamTexture.Pause ();
		}
	}

	public void Resume(OnCameraEnabled callback)
	{
		_callbackEnabled = callback;

		if(_webcamTexture != null)
		{
			_enabled = false;
			_webcamTexture.Stop ();
			_webcamTexture = null;
		}

		_ready = false;
		_enabled = true;
	}


	public void SwitchCamera(int cameraIndex)
	{
		StartCoroutine (SwitchCameraCoroutine(cameraIndex));
	}

	private IEnumerator SwitchCameraCoroutine(int cameraIndex)
	{
		if(_webcamTexture != null)
		{
			_enabled = false;
			_webcamTexture.Stop ();
			_webcamTexture = null;
		}

		yield return new WaitForSeconds (0.1f);

		_cameraIndex = cameraIndex;
		_ready = false;
		_enabled = true;
	}


	public void GetImage(OnCameraImage callback)
	{
		StartCoroutine (GetImageCoroutine (callback));
	}

	private IEnumerator GetImageCoroutine(OnCameraImage callback)
	{
		yield return new WaitForEndOfFrame();

		Texture2D texture = new Texture2D(_webcamTexture.width, _webcamTexture.height);
		
		texture.SetPixels(_webcamTexture.GetPixels());
		texture.Apply();

		if(callback != null)
		{
			callback.Invoke (texture);
		}
	}


	#region MONOBEHAVIOUR

	void Update()
	{
		if(_enabled)
		{
			if(_webcamTexture == null)
			{
				while(!Application.RequestUserAuthorization(UserAuthorization.WebCam).isDone)
				{
					return;
				}

				if (Application.HasUserAuthorization(UserAuthorization.WebCam)) 
				{
					#if UNITY_EDITOR || DEVELOPMENT_BUILD
					Debug.Log("Webcam authorized");
					#endif

					_webcamTexture = new WebCamTexture (WebCamTexture.devices[_cameraIndex].name);
					_webcamTexture.filterMode = FilterMode.Trilinear;
					_webcamTexture.Play ();	
				} 
				else 
				{
					#if UNITY_EDITOR || DEVELOPMENT_BUILD
					Debug.Log("Webcam NOT authorized");
					#endif

					if(_callbackEnabled != null)
					{
						_callbackEnabled.Invoke (false, "Webcam not authorized");
						_callbackEnabled = null;
					}
				}	
			}
			else if (_webcamTexture.isPlaying)
			{
				if(!_ready)
				{
					if (_webcamTexture.width < 100)
					{
						return;
					}

					_ready = true;

					if(_callbackEnabled != null)
					{
						_callbackEnabled.Invoke (true, null);
						_callbackEnabled = null;
					}
				}

				if(_webcamTexture.didUpdateThisFrame)
				{
					_aspectRatioFitter.aspectRatio =  (float)_webcamTexture.width / (float)_webcamTexture.height;

					_imageRectTransform.localEulerAngles = new Vector3 (0, 0, -_webcamTexture.videoRotationAngle);

					_image.texture = _webcamTexture;
				}
			}
		}
	}

	#endregion
}