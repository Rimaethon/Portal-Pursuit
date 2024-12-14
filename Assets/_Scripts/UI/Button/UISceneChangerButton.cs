using Managers;
using UnityEngine;

namespace Rimaethon.Runtime.UI
{
	public class UISceneChangerButton: UIButton
	{
		[SerializeField] private int sceneIndex;

		protected override void DoOnClick()
		{
			base.DoOnClick();
			SceneChangeEventArgs sceneChangeEventArgs = new SceneChangeEventArgs
			{
				SceneIndex = sceneIndex
			};

			EventManager.RaiseEvent(sceneChangeEventArgs);
		}
	}
}
