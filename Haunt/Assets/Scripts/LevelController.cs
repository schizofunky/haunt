using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
public class LevelController : MonoBehaviour {

	public GameObject[] levelNPCs = new GameObject[4];
	public GameObject player;
	public GameObject level;
	public int ecto = 50;
	public Object[] roomList;


	private GameObject _currentRoom;
	private Object _previousRoom;
	private FrightObject[] _frightObjects;

	// Use this for initialization
	void Start () {		
		
		putNPCsInseperateRooms();
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
			if(npcObject)
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

	private void putNPCsInseperateRooms()
	{
		_currentRoom = (GameObject)Instantiate(roomList[0], gameObject.transform.position,gameObject.transform.rotation);
		_frightObjects = _currentRoom.GetComponentsInChildren<FrightObject>();	
		_previousRoom = roomList[0];
		PathNode[] nodes = _currentRoom.GetComponentsInChildren<PathNode>();
		player.transform.position = nodes[4].transform.position;

		int allocatedRoomCounter = 0;
		List<KeyValuePair<float, Object>> list = new List<KeyValuePair<float, Object>>();

		foreach (Object room in roomList)
		{
		    list.Add(new KeyValuePair<float, Object>(Random.value, room));
		}

		var sorted = from item in list
	     orderby item.Key
	     select item;
		// Allocate new string array
		Object[] result = new Object[roomList.Length];
		// Copy values to array
		int index = 0;
		foreach (KeyValuePair<float, Object> pair in sorted)
		{
		    result[index] = pair.Value;
		    index++;
		}

		foreach(GameObject npcObject in levelNPCs)
		{
			if(npcObject.activeInHierarchy)
			{
				npcObject.GetComponent<NPC>().SetCurrentRoom((GameObject)result[allocatedRoomCounter]);		
				if(result[allocatedRoomCounter] == roomList[0])
				{
					npcObject.GetComponent<NPC>().SetCurrentPathNode(nodes[4]);	
				}			
				allocatedRoomCounter++;
			}
		}
	}

	public void ChangeCurrentRoom(Object room)
	{
		//player.SetActive(false);
		Destroy(_currentRoom);
		_currentRoom = (GameObject)Instantiate(room, gameObject.transform.position,gameObject.transform.rotation);	
		_frightObjects = _currentRoom.GetComponentsInChildren<FrightObject>();
		print(_frightObjects.Length);
		PathNode[] nodes = _currentRoom.GetComponentsInChildren<PathNode>();
		foreach(PathNode node in nodes)
		{
			if(node.type == "Door")
			{
				if(node.nodeToOtherRoom == _previousRoom)
				{
					player.transform.position = node.attachedNodes[0].transform.position;	
				}
			}
		}	
		foreach(GameObject npcObject in levelNPCs)
		{
			NPC npc = npcObject.GetComponent<NPC>();
				
			if(npc.GetCurrentRoom() == (GameObject)room) 
			{
				if(!npc.HasCurrentPathNode())
				{
					npc.SetCurrentPathNode(nodes[4]);	
				}
				npcObject.SetActive(true);
				npc.SetFrightObjects(_frightObjects);
			}
			else
			{
				npcObject.SetActive(false);
			}
		}
		_previousRoom = room;	
	}
}
