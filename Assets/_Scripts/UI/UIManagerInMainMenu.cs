using System;
using _Scripts.UI.UI_Page_View;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
	public class UIManagerInMainMenu: MonoBehaviour
	{
		[SerializeField]
		private Button background;
		private UIPage currentPage;

		private void OnEnable()
		{
			background.onClick.AddListener(() =>
			{
				CursorStateChangeEventArgs cursorStateChangeEventArgs = new CursorStateChangeEventArgs
				{
					CursorState = CursorState.IN_GAME
				};
				EventManager.RaiseEvent(cursorStateChangeEventArgs);
			});
			EventManager.Subscribe<UICloseCurrentPageEventArgs>(CloseCurrentPage);
			EventManager.Subscribe<UIOpenPageEventArgs>(OpenPage);
			EventManager.Subscribe<PauseEventArgs>(ChangeToMenuModeCursor);
		}

		private void ChangeToMenuModeCursor(PauseEventArgs obj)
		{
			CursorStateChangeEventArgs cursorStateChangeEventArgs = new CursorStateChangeEventArgs
			{
				CursorState = CursorState.MENU
			};
			EventManager.RaiseEvent(cursorStateChangeEventArgs);
		}

		private void OnDisable()
		{
			EventManager.UnSubscribe<UICloseCurrentPageEventArgs>(CloseCurrentPage);
			EventManager.UnSubscribe<UIOpenPageEventArgs>(OpenPage);
		}

		private void ChangeToMenuModeCursor()
		{
			CursorStateChangeEventArgs cursorStateChangeEventArgs = new CursorStateChangeEventArgs
			{
				CursorState = CursorState.MENU
			};
			EventManager.RaiseEvent(cursorStateChangeEventArgs);
		}

		private void CloseCurrentPage(UICloseCurrentPageEventArgs args)
		{
			if (currentPage == null) return;
			currentPage.gameObject.SetActive(false);
			currentPage = null;
		}

		private void OpenPage(UIOpenPageEventArgs args)
		{
			if (currentPage != null)
			{
				currentPage.gameObject.SetActive(false);
			}
			currentPage = args.Page;
			currentPage.gameObject.SetActive(true);
		}
	}
}
