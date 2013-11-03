using UnityEngine;
using System.Collections;

public class FrightObject : MonoBehaviour {
	public string description = "New Fright Object";
	public int frightPoints = 0;
	public int ectoRequired = 0;
	public float activationDistance = 10;
	private bool _activated;

	private int _activationCooldown = 400;

	// Use this for initialization
	void Start () {
		_activated = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!_activated)
		{
			if(_activationCooldown-- == 0)
			{
				_activated = true;
				_activationCooldown = 400;
			}

		}
	}

	public bool isCharacterWithinRange(GameObject characterObject)
	{
		float distance = Vector3.Distance(gameObject.transform.position, characterObject.transform.position);
		//print(description + " : " +distance);
		return distance < activationDistance;
	}

	public void activateFrightObject()
	{
		_activated = true;
	}

	public bool isObjectAtivated()
	{
		return _activated;
	}

	public void playFrightAnimation()
	{		
		Animation[] anims = gameObject.GetComponentsInChildren<Animation>();
		foreach(Animation anim in anims)
		{
			anim.Play();
		}
		_activated = false;
	}
}
