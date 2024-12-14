using Managers;

namespace Rimaethon.Runtime.UI
{
	public class UIBackButton:UIButton
	{
		protected override void DoOnClick()
		{
			base.DoOnClick();
			EventManager.RaiseEvent(new UICloseCurrentPageEventArgs());
		}
	}
}
