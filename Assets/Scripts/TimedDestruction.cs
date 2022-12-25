using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestruction : MonoBehaviour {
    /// <summary>
    /// destroys game object, currently used on warranty sign after flying out 
    /// </summary>
    
    public float time;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        time -= Time.deltaTime;
        if (time <= 0)
            Destroy(gameObject);
	}
}
