using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Controller
{
    public class PlayerMovement : MonoBehaviour
    {
        InputAction inputAction;
        
        public InputManager inputManager;
      
        
        
        
        
    }
    
    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        DoubleJumping,
        Falling,
        Dead
    }
}
