using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour {

	public bool canStopHere  = true;
	public GameObject[] attachedNodes;
	public int waitTimeAtNode = 300;

	public GameObject _currentOccupant;
	private PathNode _prevNode;
	private int _waitTime;


	// Use this for initialization
	void Start () {
		_waitTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(IsOccupied())
		{
			if(_waitTime++ > waitTimeAtNode)
			{
				_waitTime = 0;
				MoveToNextNode();	
			}
		}
	}

	public bool IsOccupied()
	{
		return _currentOccupant != null;
	}

	public void MoveToNextNode()
	{
		PathNode[] availableNodes = new PathNode[attachedNodes.Length];
		int availableNodeCount = 0;
		foreach(GameObject node in attachedNodes)
		{
			PathNode pathNode = node.GetComponent<PathNode>();	
			if(!pathNode.IsOccupied() && pathNode != _prevNode)
			{
				availableNodes[availableNodeCount] = pathNode;
				availableNodeCount++;
			}
		}
		if(availableNodeCount > 0)
		{
			int chosenNode = (int)Mathf.Round(Random.value * (availableNodeCount-1));
			availableNodes[chosenNode].OccupyNode(this,_currentOccupant);
			_currentOccupant = null;	
		}
		else
		{
			if(_prevNode != null && !_prevNode.IsOccupied())
			{
				_prevNode.OccupyNode(this,_currentOccupant);
				_currentOccupant = null;		
			}
			else
			{
				//no nodes around the current one are vacant so we just wait...
			}
		}
	}

	public void OccupyNode(PathNode previousNode,GameObject newOccupant)
	{
		_prevNode = previousNode;
		_currentOccupant = newOccupant;
		UnityTween tween = newOccupant.GetComponent<UnityTween>();
		TweenPositionObject tweenPosition = new TweenPositionObject();
		tweenPosition.tweenValue = gameObject.transform.position;
		tweenPosition.totalTime = 1;
		tween.TweenPosition(tweenPosition);
		_waitTime = 0;
	}
}
