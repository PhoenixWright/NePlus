using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NePlus.Global
{
    public static class Enums
    {
        public enum Action
        {            
            Action,
            Back,
            Down,
            Exit,
            JumpOrAccept,
            Left,
            Pause,
            ResetCamera,
            Right,
            Up,
            ZoomIn,
            ZoomOut
        };

        /// <summary>
        /// represents key states that are useful to know
        /// </summary>
        public enum KeyState
        {
            JustPressed,  // key went down this frame
            JustReleased, // key went up this frame
            Pressed,      // key is still down this frame
            Released      // key is still up this frame            
        };
    }
}
