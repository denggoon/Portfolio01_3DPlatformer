using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;

public class FMODSoundManager : MonoBehaviour
{
	private static FMODSoundManager instance_;

	public static FMODSoundManager instance
	{
		get
		{
			return instance_;
		}
	}

	void OnDestoy()
	{
		Debug.Log ("Destoyed!");
		StopBGM ();
		StopCondBGM ();
		StopAmbient ();
		instance_ = null;
	}

	new public Transform transform;

	private FMOD.Studio.EventInstance BGMsource_;
	private FMOD.Studio.EventInstance condBGMsrc_;
	private FMOD.Studio.EventInstance ambient_;
	
	public bool bPlayNullSound = true;
	public string missingSoundStr = "";

	public bool MasterBGMAmbientMute = false;
	public bool MasterSFXMute = false;
	
	public float MasterBGMAmbientVolume = 1.0F;
	public float MasterSFXVolume = 1.0F;

	void Awake() 
	{
		instance_ = this;
		transform = GetComponent<Transform>();

//		FMOD.Studio.System fmodStudioSys;
//		FMOD.System fmodLowLvSys;
//
//		FMOD.RESULT studioSysResult = FMOD.Studio.System.create(out fmodStudioSys);
//		FMOD.RESULT sysResult = fmodStudioSys.getLowLevelSystem(out fmodLowLvSys);
//
//		FMOD.OUTPUTTYPE outType;
//
//		FMOD.RESULT result = fmodLowLvSys.getOutput(out outType);
//
//		Debug.Log("###########Prev FMOD Output Type: " + outType.ToString());
//
//		if (Application.platform == RuntimePlatform.Android) 
//		{
//			fmodLowLvSys.setOutput (FMOD.OUTPUTTYPE.OPENSL);
//
//			FMOD.RESULT newResult = fmodLowLvSys.getOutput(out outType);
//
//			Debug.Log("###########New FMOD Output Type: " + outType.ToString());
//		}
	}

	void OnDestroy()
	{
		if(BGMsource_ != null)
			BGMsource_.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		if(ambient_ != null)
			ambient_.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		if(condBGMsrc_ != null)
			condBGMsrc_.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		instance_ = null;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.CapsLock))
		{
			bPlayNullSound = !bPlayNullSound;
		}
	}
	
	public EventInstance GetAmbient()
	{
		return this.ambient_;
	}
	
	public EventInstance GetBGM()
	{
		return this.BGMsource_;
	}

	public EventInstance GetCondBGM()
	{
		return this.condBGMsrc_;
	}
	public void BGMMute(bool isMute)
	{
		FMOD.Studio.Bus bgmMuteBus = null;

		FMOD_StudioSystem.instance.System.getBus ("bus:/Music", out bgmMuteBus);

		if (bgmMuteBus != null) {
			bgmMuteBus.setMute (isMute);
		} else {
			Debug.Log("no bgm bus found");
		}
	}
	
	public void AmbientMute(bool isMute)
	{
		FMOD.Studio.Bus ambMuteBus = null;

		FMOD_StudioSystem.instance.System.getBus ("bus:/Ambience", out ambMuteBus);
		
		if (ambMuteBus != null) {
			ambMuteBus.setMute (isMute);
		} else {
			Debug.Log("no amb bus found");
		}
	}
	
	public void EffectMute(bool isMute)
	{
		MasterSFXMute = isMute;

		FMOD.Studio.Bus sfxMuteBus = null;

		FMOD_StudioSystem.instance.System.getBus ("bus:/FX", out sfxMuteBus);

		if (sfxMuteBus != null) {
			sfxMuteBus.setMute (isMute);
		} else {
			Debug.Log("no sfx bus found");
		}

	}
	
	public void SoundMute( bool isMute ) 
	{
		this.MasterBGMAmbientMute = isMute;
		this.BGMMute( isMute );
		this.AmbientMute( isMute );
		
		this.MasterSFXMute = isMute;
		this.EffectMute( isMute );
	}

	public EventInstance PlaySoundAndReturnEvent(string path, GameObject obj)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return null;
		}

		EventInstance fmodEvent;
		if (path.Contains ("{")) 
		{
			fmodEvent = FMOD_StudioSystem.instance.GetEvent(path);
		} else {
			fmodEvent = FMOD_StudioSystem.instance.GetEvent("event:/" + path);
		}
				
		if (fmodEvent != null) 
		{
			ATTRIBUTES_3D attrib3D = FMOD.Studio.UnityUtil.to3DAttributes (obj);
			fmodEvent.set3DAttributes(attrib3D);
			fmodEvent.start ();
			return fmodEvent;
		} else {
			Debug.LogError("FMOD BGM Event: [" + path + "] does not exist");
		}

		return null;
	}
	
	public void PlayBGMSound(string path)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		BGMsource_ = FMOD_StudioSystem.instance.GetEvent("event:/" + path);

		if (BGMsource_ != null) 
		{
			BGMsource_.start ();
		} else {
			Debug.LogError("FMOD BGM Event: [" + path + "] does not exist");
		}
	}

	public void StopBGM(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		if (BGMsource_ != null) 
		{
			BGMsource_.stop(mode);
		}
	}


	public void PlayOverlapAmbient(string path, bool isLoop)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		ambient_ = FMOD_StudioSystem.instance.GetEvent("event:/" + path);

		if (ambient_ != null) 
		{
			ambient_.start ();
		} else {
			Debug.LogError("FMOD Ambient Event: [" + path + "] does not exist");
		}
	}

	public void StopAmbient(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		if (ambient_ != null) 
		{
			ambient_.stop(mode);
		}
	}

	public void PlayCondBGMSound(string path)
	{
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}
		
		condBGMsrc_ = FMOD_StudioSystem.instance.GetEvent("event:/" + path);
		
		if (condBGMsrc_ != null) 
		{
			condBGMsrc_.start ();
		} else {
			Debug.LogError("FMOD Conditional BGM Event: [" + path + "] does not exist");
		}
	}

	public void StopCondBGM(FMOD.Studio.STOP_MODE mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		if (condBGMsrc_ != null) 
		{
			condBGMsrc_.stop (mode);
		}
	}
	
	public void PlayOneShotAtPoint(string path , Vector3 pos)
	{
		if (MasterSFXMute == true) return;
		if (path == "")
		{
			Debug.LogWarning("Has not set any sound path to this action. Track down where it came from!");
			this.missingSoundStr = "No Path";
			return;
		}

		if (path.Contains ("{")) 
		{
			FMOD_StudioSystem.instance.PlayOneShot (path, pos); //GUID 재생방식 
		} else {
			FMOD_StudioSystem.instance.PlayOneShot ("event:/" + path, pos);
		}
	}

	public void SetBGMVolume(float val)
	{

	}
	
	public void SetAmbientVolume(float val)
	{

	}
	
	public void SetSFXVolume(float val)
	{
		this.MasterSFXVolume = val;
	}
}