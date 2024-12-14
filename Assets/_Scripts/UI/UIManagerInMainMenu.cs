using System;
using _Scripts.UI.UI_Page_View;
using Managers;
using UnityEngine;

namespace _Scripts.UI
{
	public class UIManagerInMainMenu: MonoBehaviour
	{
		private UIPage currentPage;

		private void OnEnable()
		{
			EventManager.Subscribe<UICloseCurrentPageEventArgs>(CloseCurrentPage);
			EventManager.Subscribe<UIOpenPageEventArgs>(OpenPage);
		}

		private void OnDisable()
		{
			EventManager.UnSubscribe<UICloseCurrentPageEventArgs>(CloseCurrentPage);
			EventManager.UnSubscribe<UIOpenPageEventArgs>(OpenPage);
		}

		private void Awake()
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
