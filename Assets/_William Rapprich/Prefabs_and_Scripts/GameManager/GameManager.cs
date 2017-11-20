using UnityEngine;
using UnityEngine.SceneManagement;

 // Author: William Rapprich
 //Last edited 20.11.2017 by William

 /// <summary>
 /// Only used for reloading the scene in this demo.
 /// </summary>
public class GameManager : MonoBehaviour
{
	public static void Reset()
	{
		SceneManager.LoadScene("Bewerbung");
	}
}
