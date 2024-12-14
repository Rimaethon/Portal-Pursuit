using _Scripts.UI.UI_Page_View;
using Managers;
using UnityEngine;

namespace Rimaethon.Runtime.UI
{
	public class UIOpenPageButton:UIButton
	{
		[SerializeField] private UIPage pageToOpen;
		protected override void DoOnClick()
		{
			base.DoOnClick();
			UIOpenPageEventArgs uiOpenPageEventArgs = new UIOpenPageEventArgs
			{
				Page = pageToOpen
			};
			EventManager.RaiseEvent(uiOpenPageEventArgs);
		}
	}
}
