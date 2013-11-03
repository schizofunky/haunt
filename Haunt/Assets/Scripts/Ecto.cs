using UnityEngine;
using System.Collections;

public class Ecto : MonoBehaviour {
	public int amount = 5;
	public float decay = 0.2f;

	private float _life;

	// Use this for initialization
	void Start () {
		_life = 100;
	}
	
	// Update is called once per frame
	void Update () {
		_life -= decay;
		if(_life <= 0)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider collider){
		if(collider.name == "Polterguy")
		{
			Destroy(gameObject.transform.parent.gameObject);
			GameObject.Find("LevelController").SendMessage("UpdateEctoCount",amount);
		}
	}
}
