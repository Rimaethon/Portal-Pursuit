using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerInGame : MonoBehaviour
{
    [SerializeField] Button resumeButton;
    [SerializeField] GameObject pausePage;
    [SerializeField] GameObject winPage;
    [SerializeField] GameObject losePage;

    private void OnEnable()
    {
        resumeButton.onClick.AddListener(() =>
        {
            pausePage.SetActive( false );
            ChangeToMenuModeCursor(CursorState.IN_GAME);
            Time.timeScale = 1;
        });
        EventManager.Subscribe<PauseEventArgs>(HandlePausePage);
        EventManager.Subscribe<PlayerWinEventArgs>(HandleWinPage);
        EventManager.Subscribe<PlayerLoseEventArgs>(HandleLosePage);
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveAllListeners();
        EventManager.UnSubscribe<PauseEventArgs>(HandlePausePage);
        EventManager.UnSubscribe<PlayerWinEventArgs>(HandleWinPage);
        EventManager.UnSubscribe<PlayerLoseEventArgs>(HandleLosePage);
    }

    private void Awake()
    {
        ChangeToMenuModeCursor(CursorState.IN_GAME);
    }

    private void HandlePausePage(PauseEventArgs obj)
    {
        pausePage.SetActive( true );
        ChangeToMenuModeCursor(CursorState.MENU);
        Time.timeScale = 0;
    }

    private void ChangeToMenuModeCursor(CursorState cursorState)
    {
        CursorStateChangeEventArgs cursorStateChangeEventArgs = new CursorStateChangeEventArgs
        {
            CursorState =cursorState
        };
        EventManager.RaiseEvent(cursorStateChangeEventArgs);
    }

    private void HandleWinPage(PlayerWinEventArgs obj)
    {
        winPage.SetActive( true );
        Time.timeScale = 0;
    }

    private void HandleLosePage(PlayerLoseEventArgs obj)
    {
        losePage.SetActive( true );
        Time.timeScale = 0;
    }
}
