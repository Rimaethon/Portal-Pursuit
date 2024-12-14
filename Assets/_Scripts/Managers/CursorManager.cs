using Managers;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public CursorState CursorState => cursorState;
    private CursorState cursorState = CursorState.MENU;

    private void OnEnable()
    {
        EventManager.Subscribe<CursorStateChangeEventArgs>(ChangeCursorMode);
    }

    private void OnDisable()
    {
        EventManager.UnSubscribe<CursorStateChangeEventArgs>(ChangeCursorMode);
    }

    private void ChangeCursorMode(CursorStateChangeEventArgs args)
    {
        switch (args.CursorState)
        {
            case CursorState.IN_GAME:
                cursorState = CursorState.IN_GAME;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case CursorState.MENU:
                cursorState = CursorState.MENU;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
        }
    }

}

public enum CursorState
{
    IN_GAME,
    MENU
}
