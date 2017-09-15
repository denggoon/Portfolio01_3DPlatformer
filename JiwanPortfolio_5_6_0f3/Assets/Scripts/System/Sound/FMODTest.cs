using UnityEngine;
using System.Collections;

public class FMODTest : MonoBehaviour {

	FMOD.Studio.EventInstance fmodEvent;

	// Use this for initialization
	void Start () {
		fmodEvent = FMOD_StudioSystem.instance.GetEvent ("event:/Monster/Bo_Shoot_Move");

		fmodEvent.start ();
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Delete))
		{
			FMOD.Studio.CueInstance cue;

			fmodEvent.getCue("KeyOff", out cue);

			cue.trigger();
		}
	}
}
