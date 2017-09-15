using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public float maxHealth = 2F;
	public float currHealth = 2F;

	public bool isInvincible = false;
	public bool isItemInvincible = false;
	Coroutine invincibleCoroutine;

	public float invincibleTime = 3F;
	public float invincibleTimer;

	public SkinnedMeshRenderer[] invincibleBody;

	public UnityEngine.AI.NavMeshObstacle navMeshObs;

	void Awake()
	{
		invincibleBody = GetComponentsInChildren<SkinnedMeshRenderer> ();
	}

	void Start()
	{
		Revive();

		navMeshObs = GetComponent<UnityEngine.AI.NavMeshObstacle>();

		int intHealth = System.Convert.ToInt32 (currHealth);

		for (int i=0; i<intHealth; i++) 
		{
			UIManager.instance.AddHealth();
		}
	}

	public void Revive()
	{
		if (PurchasedItemEffect.instance != null) 
		{
			currHealth = maxHealth + PurchasedItemEffect.instance.GetItemEffectValue(PURCHASED_ITEM.HEALTH_BOOST);

			if(PurchasedItemEffect.instance.GetItemEffectValue(PURCHASED_ITEM.MAGNETIC) == 1)
			{
				GameRuleManager.instance.playerMove.AddPassive(System.Convert.ToInt32(PLAYER_PASSIVE.PlayerPassive_Magnet));
			}

		} else {
			currHealth = maxHealth;
		}
	}

	public void AddHealth(int point)
	{
		if (currHealth < maxHealth) 
		{
			currHealth += point;

			UIManager.instance.AddHealth();

		} else {
			GameRuleManager.instance.AddGold(10);
		}
	}

	public void TakeDamage(float damage)
	{
		if (GameRuleManager.instance.eGameStatus != E_GAME_STATUS.IN_PLAY) 
		{
			if (damage == 999F) 
			{
				Destroy (GameRuleManager.instance.playerMove.mainTransform.gameObject);
				GameRuleManager.instance.player = null;
			}

			return;
		}

		if(isInvincible == true)
		{
			if(damage != 999F)
				return;
		}

		if(SoundBoard.instance != null)
			SoundBoard.instance.PlayFromSoundBoard("SND_PC_DAMAGE", this.transform.position);

		currHealth -= damage;

		GameRuleManager.instance.playerMove.RemovePassive (System.Convert.ToInt32 (PLAYER_PASSIVE.PlayerPassive_Magnet));


		UIManager.instance.LoseHealth ();
//		UIManager.instance.playerHealth = (int)this.currHealth;

		if(currHealth <= 0F)
		{
			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_PC_DAMAGE_DEATH", this.transform.position);

			GameRuleManager.instance.playerMove.animator.SetTrigger("DeadTgr");
			if(damage == 999F)
			{
				Destroy(GameRuleManager.instance.playerMove.mainTransform.gameObject);
				GameRuleManager.instance.player = null;
			} else {
				StartInvincible();
			}

			GameRuleManager.instance.GameOver();

		} else {
			GameRuleManager.instance.playerMove.animator.SetTrigger("DamageTgr");
			StartInvincible();
		}
	}

	public void StartInvincible(float timeValue = -1F, bool isItemTriggered = false)
	{
		CancelInvincible ();

		if(timeValue == -1F)
			timeValue = invincibleTime;

		invincibleCoroutine = StartCoroutine(BeInvincible(timeValue, isItemTriggered));
	}

	public void CancelInvincible()
	{
		if(invincibleCoroutine != null)
			StopCoroutine(invincibleCoroutine);
	}

	IEnumerator BeInvincible(float time, bool isItemTrigerred = false)
	{
		isInvincible = true;

		if (isItemTrigerred) 
		{
			if(SoundBoard.instance != null)
				SoundBoard.instance.PlayFromSoundBoard("SND_BGM_ITEM_INVINCIBLE");

			isItemInvincible = true;
		}
//		navMeshObs.enabled = false;

		invincibleTimer = time;

		while(invincibleTimer > 0F)
		{
			invincibleTimer -= Time.deltaTime;

			for(int i=0; i<invincibleBody.Length; i++)
			{
				invincibleBody[i].enabled = !invincibleBody[i].enabled;
			}

			yield return null;
		}

		for(int i=0; i<invincibleBody.Length; i++)
		{
			invincibleBody[i].enabled = true;
		}

		isInvincible = false;

		if (isItemTrigerred) 
		{
			if(FMODSoundManager.instance != null)
				FMODSoundManager.instance.StopCondBGM ();

			isItemInvincible = false;
		}
//		navMeshObs.enabled = true;
	}
}
