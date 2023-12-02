using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class InputManager : MonoBehaviour
    {
        
        // Properties
        public MoveDirection UIMoveDirection { get; set; } // For the Button Controller to Set
        
        // Fields
        private Player player; // init by Start

        private void Start()
        {
            player = GameManager.Instance.Player;
        }

        /// <summary>
        /// Returns the input from the player, prioritizing the UI Input.
        /// </summary>
        /// <returns>Player Movement Direction; None if there wasn't any</returns>
        public MoveDirection GetInput()
        {
            var kbInput = TakeKeyboardInput();
            var uiInput = UIMoveDirection;

            // Reset the UI input
            UIMoveDirection = MoveDirection.None; 

            // If UI input is not None, return it, otherwise return keyboard input, which if there's no keyboard input will be none
            return uiInput != MoveDirection.None ? uiInput : kbInput;
        }
        
        /// <summary>
        /// Take axis input and ensure only one is used
        /// </summary>
        /// <returns>Keyboard Directional Input; None if there wasn't any</returns>
        private MoveDirection TakeKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                return MoveDirection.Right;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                return MoveDirection.Left;
            if (Input.GetKeyDown(KeyCode.UpArrow))
                return MoveDirection.Up;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                return MoveDirection.Down;

            return MoveDirection.None;
        }
    }

}
