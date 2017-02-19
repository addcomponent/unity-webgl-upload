using UnityEngine;
using System.Collections;

public class UILoadingIcon : MonoBehaviour
{
	[SerializeField]
	private float _speed;

	void Update ()
	{
		transform.Rotate (Vector3.forward * -_speed * Time.deltaTime);
	}
}
