using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour {

	public bool canStopHere  = true;
	public GameObject[] attachedNodes;
	public string type = "Normal";
	public GameObject nodeToOtherRoom;

	private PathNode _prevNode;
	private PathNode[] _currentFastestRoute;
	private PathNode[] _exitRoute;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other){
		GameObject.Find("LevelController").SendMessage("ChangeCurrentRoom",nodeToOtherRoom);
	}

	public PathNode MoveToNextNode(NPC currentNPC)
	{
		PathNode nextNode = this;
		if(currentNPC.GetCurrentPathArray() != null)
		{
			PathNode[] currentPath = unshiftPaths(currentNPC.GetCurrentPathArray());
			currentNPC.SetCurrentPath(currentPath);

			//cycle through the paths to find the next valid one and move to it
			nextNode = GetDestinationNode(currentPath);
			if(nextNode.type == "Exit" && !currentNPC.hasSanityLeft())
			{
				currentNPC.MakeNPCRunOutOfHouse();
			}
			else if(nextNode.type == "Door")
			{
				PathNode nodeDoorOfNextRoom = nextNode.nodeToOtherRoom.GetComponent<PathNode>();
				nextNode = nodeDoorOfNextRoom;	
				currentNPC.SetMovedToNewRoom();	
			}
			if(nextNode != null)
			{
				nextNode.OccupyNode(this,currentNPC);				
			}
		}
		else
		{
			bool movePlayerToDoor = currentNPC.wantsToLeaveRoom();
			if(!currentNPC.hasSanityLeft())
			{
				PathNode[] route = {this};
				checkNeighbourNodes(route,this,"Exit");
				if(_exitRoute != null)
				{
					currentNPC.SetCurrentPath(_exitRoute);
					nextNode = MoveToNextNode(currentNPC);
				}
				else
				{
					//Wants to exit but no exit in this room, so move em to another room
					movePlayerToDoor = true;
				}
				//player really wants to leave house
				//check if an exit door exists and map it
			}
			if(movePlayerToDoor)
			{
				//TODO: The generated exit path never changes for the node, maybe just call it once?
				PathNode[] route = {this};
				if(this.type == "Door")
				{
					currentNPC.SetCurrentPath(route);
					nextNode = MoveToNextNode(currentNPC);
				}
				else
				{					
					if(_currentFastestRoute == null)
					{
						checkNeighbourNodes(route,this,"Door");
					}
					if(_currentFastestRoute != null)
					{
						currentNPC.SetCurrentPath(_currentFastestRoute);
						nextNode = MoveToNextNode(currentNPC);
					}
					else
					{
						print("ERROR!: "+currentNPC.npcName +" wants to change rooms and route is "+_currentFastestRoute);
					}
				}
			}
			else
			{
				nextNode = MoveToRandomNode(currentNPC);
			}			
		}
		return nextNode;
	}

	private PathNode GetDestinationNode(PathNode[] currentPath)
	{
		foreach(PathNode path in currentPath)
		{
			if(path.type == "Door")
			{
				//move to that node then!
				//remember to remove the set path for the npc
				return path;
			}
			else if(path != this)
			{
				//move to that node
				return path;
			}
		}
		return currentPath[currentPath.Length-1];
	}

	private PathNode MoveToRandomNode(NPC currentNPC)
	{
		PathNode[] availableNodes = new PathNode[attachedNodes.Length];
		PathNode nextNode;
		int availableNodeCount = 0;
		foreach(GameObject node in attachedNodes)
		{
			PathNode pathNode = node.GetComponent<PathNode>();	
			if(pathNode != _prevNode)
			{
				availableNodes[availableNodeCount] = pathNode;
				availableNodeCount++;
			}
		}
		if(availableNodeCount > 0)
		{
			int chosenNode = (int)Mathf.Round(Random.value * (availableNodeCount-1));
			nextNode = availableNodes[chosenNode];
		}
		else
		{
			if(_prevNode != null)
			{
				nextNode = _prevNode;
			}
			else
			{
				nextNode = this;
				//no nodes around the current one are vacant so we just wait...
			}
		}
		nextNode.OccupyNode(this,currentNPC);
		return nextNode;
	}

	public PathNode[] copyArraySpecial(PathNode[] originalArray)
	{
		PathNode[] copiedArray = new PathNode[originalArray.Length+1];
		for(int i =0; i < originalArray.Length; i++)
		{
			copiedArray[i] = originalArray[i];
		}
		return copiedArray;
	}

	public PathNode[] unshiftPaths(PathNode[] paths)
	{
		if(paths.Length == 1)
		{
			return paths;
		}
		PathNode[] unshiftedPaths = new PathNode[paths.Length-1];
		for(int i = 1; i < paths.Length; i++)
		{
			unshiftedPaths[i-1] = paths[i];
		}
		return unshiftedPaths;
	}

	public void OccupyNode(PathNode previousNode,NPC currentNPC)
	{
		_prevNode = previousNode;
		UnityTween tween = currentNPC.gameObject.GetComponent<UnityTween>();
		TweenPositionObject tweenPosition = new TweenPositionObject();
		tweenPosition.tweenValue = gameObject.transform.position;
		tweenPosition.totalTime = 1;
		tween.TweenPosition(tweenPosition);
	}

	public void checkNeighbourNodes(PathNode[] route, PathNode currentNode, string desiredNode)
	{
		foreach(GameObject nodeObject in currentNode.attachedNodes)
		{
			PathNode node = nodeObject.GetComponent<PathNode>();
			if(System.Array.IndexOf(route,node) == -1)
			{
				if(node.type == desiredNode)
				{
					PathNode[] routeClone = copyArraySpecial(route);
					routeClone[routeClone.Length-1] = node;
					if(desiredNode == "Door")
					{
						storeFastestRouteToDoorNode(routeClone);
					}
					if(desiredNode == "Exit")
					{
						storeFastestRouteToExitNode(routeClone);
					}
				}
				else
				{
					PathNode[] routeClone = copyArraySpecial(route);
					routeClone[routeClone.Length-1] = node;
					checkNeighbourNodes(routeClone,node,desiredNode);	
				}				
			}
		}	
	}

	private void storeFastestRouteToDoorNode(PathNode[] route)
	{
		if(_currentFastestRoute == null)
		{
			_currentFastestRoute = route;
		}
		else
		{
			if(_currentFastestRoute.Length > route.Length)
			{
				_currentFastestRoute = route;	
			}
		}
	}

	private void storeFastestRouteToExitNode(PathNode[] route)
	{
		if(_exitRoute == null)
		{
			_exitRoute = route;
		}
		else
		{
			if(_exitRoute.Length > route.Length)
			{
				_exitRoute = route;	
			}
		}
	}
}
