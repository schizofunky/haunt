using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public string npcName = "New NPC";
	public int sanityPoints = 100;
	public int scepticLevel = 1;
	public GameObject currentRoom;
	private int _ectoEarned;
	private int _roomInterest;

	// Use this for initialization
	void Start () {
		_ectoEarned = 0;
		_roomInterest = 100;
	}
	
	// Update is called once per frame
	void Update () 
	{
		FrightObject[] frightObjectsInRoom = currentRoom.GetComponentsInChildren<FrightObject>();
		foreach(FrightObject frightObject in frightObjectsInRoom)
		{
			if(frightObject.isCharacterWithinRange(gameObject) && frightObject.isObjectAtivated())
			{
				frightObject.playFrightAnimation();
				Scare(frightObject);
			}
		}
	}

	private void Scare(FrightObject frightObject)
	{
		int scarePoints = (int)Mathf.Round(frightObject.frightPoints/scepticLevel);
		sanityPoints -= scarePoints;
		//TODO: check this doesn't go below 0? maybe
		_ectoEarned += scarePoints;
		if(!hasSanityLeft())
		{
			gameObject.SetActive(false);
		}
	}

	public int getEarnedEcto()
	{
		//NB: This function may not get called after each fright, maybe when the npc leaves the room, so we reset the earned amount when it is called
		int earnedEcto = _ectoEarned;
		_ectoEarned = 0;
		return earnedEcto;
	}

	public bool hasSanityLeft()
	{
		return sanityPoints > 0;
	}

	public bool wantsToLeaveRoom()
	{
		return _roomInterest <= 0;
	}
}
