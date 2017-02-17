//
//  SocketBehaviourWebHelper.cs
//
//  Author:
//       Tomaz Saraiva <tomaz.saraiva@gmail.com>
//
//  Copyright (c) 2017 Tomaz Saraiva

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WebGLBridge : MonoBehaviour
{
	public delegate void OnLoadedImage(Texture2D texture);

	private OnLoadedImage _callback;


	public delegate void OnLogMessage(string message);

	public OnLogMessage _onLogMessage;

		
	public void SelectImage (int maxSize, OnLoadedImage callback) 
	{
		_callback = callback;

		#if UNITY_EDITOR
		string path = UnityEditor.EditorUtility.OpenFilePanel("Open image","","jpg,png,bmp");

		if (!System.String.IsNullOrEmpty (path))
		{
			FileSelected ("file:///" + path);	
		}
		#else 
		Application.ExternalCall ("SelectFile", maxSize);
		#endif
	}

	private void FileSelected (string url) 
	{
		StartCoroutine(LoadTexture (url));
	}

	private IEnumerator LoadTexture (string url) 
	{
		WWW www = new WWW (url);

		yield return www;

		Log ("Texture Loaded " + GetBytesReadable (www.size));

		if(_callback != null)
		{
			_callback.Invoke (www.texture);
			_callback = null;
		}
	}


	public void Log(string message)
	{
		if(_onLogMessage != null)
		{
			_onLogMessage.Invoke (message);
		}
	}


	private string GetBytesReadable (long i)
	{
		// Get absolute value
		long absolute_i = (i < 0 ? -i : i);
		// Determine the suffix and readable value
		string suffix;
		double readable;
		if (absolute_i >= 0x1000000000000000) // Exabyte
		{
			suffix = "EB";
			readable = (i >> 50);
		}
		else if (absolute_i >= 0x4000000000000) // Petabyte
		{
			suffix = "PB";
			readable = (i >> 40);
		}
		else if (absolute_i >= 0x10000000000) // Terabyte
		{
			suffix = "TB";
			readable = (i >> 30);
		}
		else if (absolute_i >= 0x40000000) // Gigabyte
		{
			suffix = "GB";
			readable = (i >> 20);
		}
		else if (absolute_i >= 0x100000) // Megabyte
		{
			suffix = "MB";
			readable = (i >> 10);
		}
		else if (absolute_i >= 0x400) // Kilobyte
		{
			suffix = "KB";
			readable = i;
		}
		else
		{
			return i.ToString ("0 B"); // Byte
		}
		// Divide by 1024 to get fractional value
		readable = (readable / 1024);
		// Return formatted number with suffix
		return readable.ToString ("0.### ") + suffix;
	}
}