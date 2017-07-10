using UnityEngine;
using UnityEngine.SceneManagement;

namespace TextQuest
{
	public class StartGameScript : MonoBehaviour
	{
		public void onStartGameClick()
		{
			SceneManager.LoadScene(1, LoadSceneMode.Single);
		}
	}
}
