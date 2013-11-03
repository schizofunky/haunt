using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public string npcName = "New NPC";
	public int sanityPoints = 100;
	public int scepticLevel = 1;
	public GameObject currentRoom;
	public Object ectoPrefab;
	public GameObject startingNode;
	public int waitTimeAtNode = 300;
	private int _ectoEarned;
	private int _roomInterest;
	private PathNode[] _currentPathArray;
	private PathNode _currentPathNode;
	private int _waitTime;

	// Use this for initialization
	void Start () {
		_waitTime = (int)Mathf.Round(Random.value * 300);
		_ectoEarned = 0;
		_roomInterest = 500 + (int)Mathf.Round(Random.value * 1000);
		_currentPathNode = startingNode.GetComponent<PathNode>();
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
		_roomInterest--;
		if(_waitTime++ > waitTimeAtNode)
		{
			_waitTime = (int)Mathf.Round(Random.value * 300);
			_currentPathNode = _currentPathNode.MoveToNextNode(this);
		}
	}

	private void Scare(FrightObject frightObject)
	{
		int scarePoints = (int)Mathf.Round(frightObject.frightPoints/scepticLevel);
		sanityPoints -= scarePoints;
		//TODO: check this doesn't go below 0? maybe
		_ectoEarned += scarePoints;
		GameObject ecto = (GameObject)Instantiate(ectoPrefab, gameObject.transform.position, gameObject.transform.rotation);
		ecto.GetComponentInChildren<Ecto>().amount = getEarnedEcto();
		_roomInterest = (int)Mathf.Round(_roomInterest*0.8f);
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

	public void MakeNPCRunOutOfHouse()
	{
		Destroy(gameObject);
	}

	public void SetCurrentPath(PathNode[] currentPathArray)
	{
		_currentPathArray = currentPathArray;
	}

	public PathNode[] GetCurrentPathArray()
	{
		return _currentPathArray;
	}

	public void SetMovedToNewRoom()
	{
		_currentPathArray = null;
		_roomInterest = 500 + (int)Mathf.Round(Random.value * 1000);
	}
}
