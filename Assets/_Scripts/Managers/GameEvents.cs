using System;
using _Scripts.UI.UI_Page_View;
using UnityEngine;

namespace Managers
{
	public class JumpEventArgs : EventArgs
	{
	}

	public class MusicToggleEventArgs : EventArgs
	{
	}

	public class PageChangedEventArgs : EventArgs
	{
		public GameObject page;
	}

	public class PauseEventArgs : EventArgs
	{
	}

	public class ScorePickupEventArgs : EventArgs
	{
		public int score;
	}

	public class LifePickupEventArgs : EventArgs
	{
	}
	public class HealthPickupEventArgs : EventArgs
	{
		public int healingAmount;
	}

	public class PlayerLoseEventArgs : EventArgs
	{
	}

	public class PlayerWinEventArgs : EventArgs
	{
	}

	public class SceneChangeEventArgs : EventArgs
	{
		public int SceneIndex;
	}

	public class UICloseCurrentPageEventArgs : EventArgs
	{
	}

	public class InteractEventArgs: EventArgs
	{
	}

	public class CursorStateChangeEventArgs : EventArgs
	{
		public CursorState CursorState;
	}

	public class UIOpenPageEventArgs : EventArgs
	{
		public UIPage Page;
	}
}
