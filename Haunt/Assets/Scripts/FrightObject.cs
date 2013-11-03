using UnityEngine;
using System.Collections;

public class FrightObject : MonoBehaviour {
	public string description = "New Fright Object";
	public int frightPoints = 0;
	public int ectoRequired = 0;
	public int activationDistance = 10;
	private bool _activated;

	// Use this for initialization
	void Start () {
		_activated = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool isCharacterWithinRange(GameObject characterObject)
	{
		return false;
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
		if(isObjectAtivated())
		{			
			Animation[] anims = gameObject.GetComponentsInChildren<Animation>();
			foreach(Animation anim in anims)
			{
				anim.Play();
			}
			_activated = false;
		}
	}
}
