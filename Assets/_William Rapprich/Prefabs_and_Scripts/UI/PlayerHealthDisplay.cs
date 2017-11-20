using UnityEngine;
using UnityEngine.UI;

//Author: William Rapprich
//Last edited: 20.11.2017 by: William

/// <summary>
/// Displays player health on the screen and updates the display on health change.
/// </summary>
public class PlayerHealthDisplay : MonoBehaviour
{
	public static PlayerHealthDisplay Instance { get; private set; }
	Image[] heartContainer;
	int maxHearts;
	GameObject heartPrefab;

	void Awake()
	{
		//Singleton instance
		if (Instance != null && Instance != this)
		{
			// Destroy the heretic if there is another instance
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	void Start ()
	{
		//One heart equals two HP
		maxHearts = Mathf.CeilToInt(Player.Instance.GetMaxHp()/2f);
		heartPrefab = PrefabHolder.Instance.heart;
		heartContainer = new Image[maxHearts];

		//fill canvas and container with hearts
		for(int i = 0; i < heartContainer.Length; i++)
		{
			heartContainer[i] = Instantiate(heartPrefab, transform).GetComponent<Image>();
			heartContainer[i].type = Image.Type.Filled;
			heartContainer[i].fillMethod = Image.FillMethod.Horizontal;
			heartContainer[i].fillAmount = 0f;
		}
		Player.Instance.healthChange.AddListener(Display);
		Display(Player.Instance.GetHp());
	}

	/// <summary>
	/// Displays current HP as heart halves
	/// </summary>
	/// <param name="newHealth">New current HP to be displayed</param>
	void Display(int newHealth)
	{
		if(newHealth > 0)
		{
			for(int i = 0; i < heartContainer.Length; i++)
			{
				if (i < newHealth/2 )
				{
					heartContainer[i].fillAmount = 1f;
				}
				else if(i == newHealth/2 && newHealth%2 == 1 )
				{
					heartContainer[i].fillAmount = 0.5f;
				}
				else
				{
					heartContainer[i].fillAmount = 0f;
				}
			}
		}
		else
		{
			foreach(Image hrt in heartContainer)
			{
				hrt.fillAmount = 0f;
			}
		}
	}

	void OnDestroy()
	{
		Instance = null;
	}
}
