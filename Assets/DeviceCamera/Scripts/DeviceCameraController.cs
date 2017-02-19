//
//  DeviceCamController.cs
//
//  Author:
//       Tomaz Saraiva <tomaz.saraiva@gmail.com>
//
//  Copyright (c) 2017 Tomaz Saraiva
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeviceCameraController : MonoBehaviour
{
	private const string MSG_NO_CAMERA = "No cameras available";


	[SerializeField]
	private DeviceCameraControllerUI _controllerUI;

	[SerializeField]
	private DeviceCamera _deviceCamera;


	private DeviceCamera.OnCameraImage _callback;

	private int _selectedCamera;


	public void ShowCamera(DeviceCamera.OnCameraImage callback)
	{
		if(WebCamTexture.devices.Length > 0)
		{
			_callback = callback;

			StartCamera ();
		}
		else 
		{
			_controllerUI.ShowError (MSG_NO_CAMERA);
		}

		_controllerUI.Show (true);
	}

	private void StartCamera()
	{
		_controllerUI.Loading (true);

		_deviceCamera.Enable (((bool success, string message) => 
		{
			_controllerUI.Loading (false);

			if(success)
			{
				_controllerUI.ShowTakePicture ();
			}
			else 
			{
				_controllerUI.ShowError (message);
			}
		}), _selectedCamera);

		if(WebCamTexture.devices.Length > 1)
		{
			_controllerUI.ShowSwitchButton (true);
		}
	}


	public void Capture()
	{
		_deviceCamera.Pause ();

		_controllerUI.ShowSavePicture ();
	}

	public void ResetCamera()
	{
		_deviceCamera.Resume (((bool success, string message) =>
		{
			_controllerUI.ShowTakePicture ();
		}));
	}

	public void Save()
	{
		_controllerUI.Loading (true);

		_deviceCamera.GetImage (((Texture2D texture) => 
		{
			_controllerUI.Loading (false);

			if(_callback != null)
			{
				_callback.Invoke (texture);
			}

			Close();

		}));
	}


	public void Switch()
	{
		if(_selectedCamera < WebCamTexture.devices.Length - 1)
		{
			_selectedCamera++;
		}
		else 
		{
			_selectedCamera = 0;
		}

		_deviceCamera.SwitchCamera (_selectedCamera);
	}

	public void Close()
	{
		_deviceCamera.Disable ();

		_controllerUI.Show (false);

		if(_callback != null)
		{
			_callback.Invoke (null);
		}
	}
}