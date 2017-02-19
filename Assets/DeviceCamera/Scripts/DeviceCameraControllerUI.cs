//
//  DeviceCameraControllerUI.cs
//
//  Author:
//       Tomaz Saraiva <tomaz.saraiva@gmail.com>
//
//  Copyright (c) 2017 Tomaz Saraiva
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeviceCameraControllerUI : MonoBehaviour
{
	[SerializeField]
	private GameObject _content;

	[SerializeField]
	private GameObject _goBackground;

	[SerializeField]
	private Text _labelError;

	[SerializeField]
	private GameObject _goIconFailed;

	[SerializeField]
	private GameObject _goIconLoading;

	[SerializeField]
	private Button _buttonClose;

	[SerializeField]
	private Button _buttonSwitch;

	[SerializeField]
	private Button _buttonCapture;

	[SerializeField]
	private Button _buttonReset;

	[SerializeField]
	private Button _buttonSave;


	public void Show(bool show)
	{
		_content.SetActive (show);

		if(!show)
		{
			Loading (false);

			HideError ();

			ShowButtons (false);
		}
	}

	public void Loading(bool loading)
	{
		_goIconLoading.SetActive (loading);

		if(loading)
		{
			ShowButtons (false);
		}
	}


	public void ShowError(string error)
	{
		_goIconFailed.SetActive (true);

		_labelError.text = error;
		_labelError.gameObject.SetActive (true);
	}

	public void HideError()
	{
		_goIconFailed.SetActive (false);

		_labelError.text = null;
		_labelError.gameObject.SetActive (false);
	}


	public void ShowTakePicture()
	{
		_buttonReset.gameObject.SetActive (false);
		_buttonSave.gameObject.SetActive (false);

		_buttonCapture.gameObject.SetActive (true);
	}

	public void ShowSavePicture()
	{
		_buttonCapture.gameObject.SetActive (false);

		_buttonReset.gameObject.SetActive (true);
		_buttonSave.gameObject.SetActive (true);
	}
		
	public void ShowSwitchButton(bool show)
	{
		_buttonSwitch.gameObject.SetActive (show);
	}

	public void ShowButtons(bool show)
	{
		_buttonReset.gameObject.SetActive (show);
		_buttonSave.gameObject.SetActive (show);
		_buttonCapture.gameObject.SetActive (show);
	}
}