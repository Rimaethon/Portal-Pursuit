using System.Collections;
using Managers;
using Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Managers
{
	public class SceneController:PrivatePersistentSingleton<SceneController>
	{
		private Canvas loadingCanvas;

		protected override void Awake()
		{
			base.Awake();
			loadingCanvas = GetComponentInChildren<Canvas>();
			StartCoroutine(LoadSceneAsync());
		}

		private void OnEnable()
		{
			EventManager.Subscribe<SceneChangeEventArgs>(HandleSceneChange);
		}

		private void OnDisable()
		{
			EventManager.UnSubscribe<SceneChangeEventArgs>(HandleSceneChange);
		}

		private IEnumerator LoadSceneAsync(int sceneIndex = 1)
		{
			loadingCanvas.enabled = true;
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			loadingCanvas.enabled = false;
		}

		private void HandleSceneChange(SceneChangeEventArgs args)
		{
			if (SceneManager.GetActiveScene().buildIndex != args.SceneIndex)
			{
				StartCoroutine(LoadSceneAsync(args.SceneIndex));
			}
		}
	}
}
