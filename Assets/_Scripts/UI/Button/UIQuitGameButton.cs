using UnityEngine;

namespace Rimaethon.Runtime.UI
{
	public class UIQuitGameButton:UIButton
	{
		protected override void DoOnClick()
		{
			base.DoOnClick();
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#endif
			Application.Quit();
		}
	}
}
