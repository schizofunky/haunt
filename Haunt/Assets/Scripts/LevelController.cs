using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	public GameObject[] levelNPCs = new GameObject[4];
	public GameObject player;
	public GameObject level;
	public int ecto = 50;

	private FrightObject[] _frightObjects;

	// Use this for initialization
	void Start () {		
		_frightObjects = level.GetComponentsInChildren<FrightObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(IsLevelComplete())
		{
			print("Level Completed");
		}
		else
		{
			if (Input.GetButtonDown ("Jump"))
			{
				foreach(FrightObject frightObject in _frightObjects)
				{
					if(!frightObject.isObjectAtivated() && frightObject.isCharacterWithinRange(player))
					{
						print("Activated "+frightObject.description);
						frightObject.activateFrightObject();
					}
				}
			}
		}
	
	}

	public bool IsLevelComplete()
	{
		bool complete = true;
		foreach(GameObject npcObject in levelNPCs)
		{
			if(npcObject.GetComponent<NPC>().hasSanityLeft())
			{
				complete = false;
			}
		}
		return complete;
	}

	public void UpdateEctoCount(int amount)
	{
		ecto += amount;
		print("You gained "+amount+" ecto!");
	}
}
