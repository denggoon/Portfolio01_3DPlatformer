using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using FMOD.Studio;

public class FMODSoundIDPlayer : MonoBehaviour 
{
	public string soundID;

	void Start()
	{
		if (!string.IsNullOrEmpty (soundID)) 
		{
			SoundInfo soundInfo = SoundBoard.instance.GetPlayerSoundInfo(soundID);

			if(soundInfo != null)
			{
				FMOD_StudioEventEmitter emitter = this.gameObject.AddComponent<FMOD_StudioEventEmitter>();
				emitter.path = "event:/" + soundInfo.eventPath;
			} else {
				Debug.Log("null");
			}
		}
	}
}
