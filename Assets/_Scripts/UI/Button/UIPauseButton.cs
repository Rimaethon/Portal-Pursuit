using Managers;

namespace Rimaethon.Runtime.UI
{
	//Actually this button can pause or unpause the game depending on the current state
	public class UIPauseButton:UIButton
	{
		protected override void DoOnClick()
		{
			base.DoOnClick();
			EventManager.RaiseEvent(new PauseEventArgs());
		}
	}
}
