using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {

	public Transform TextTarget;

	// Use this for initialization
	void Start () {
		Destroy(gameObject,3);
	}

	public IEnumerator destoryTextTarget()
	{
		yield return new WaitForSeconds(3);
		if(TextTarget!=null)
			Destroy(TextTarget.gameObject);
	}
}
