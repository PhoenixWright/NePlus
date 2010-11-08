using Microsoft.Xna.Framework.Input;

namespace NePlus.EngineComponents
{
    public class Configuration
    {
        // debug view
        public bool ShowDebugView { get; private set; }


        #region Menu Actions
        // accept (confirm)
        public Buttons AcceptButton { get; private set; }
        public Keys AcceptKey { get; private set; }

        // back (reject/deny)
        public Buttons BackButton { get; private set; }
        public Keys BackKey { get; private set; } 
        #endregion

        #region Movement
        // down
        public Buttons DownButton { get; private set; }
        public Keys DownKey { get; private set; }

        // left
        public Buttons LeftButton { get; private set; }
        public Keys LeftKey { get; private set; }

        // right
        public Buttons RightButton { get; private set; }
        public Keys RightKey { get; private set; }

        // up
        public Buttons UpButton { get; private set; }
        public Keys UpKey { get; private set; } 
        #endregion

        #region Game Actions
        // jump
        public Buttons JumpButton { get; private set; }
        public Keys JumpKey { get; private set; }

        // quit
        public Buttons QuitButton { get; private set; }
        public Keys QuitKey { get; private set; }        
        #endregion

        #region Camera
        // reset camera
        public Buttons ResetCameraButton { get; private set; }
        public Keys ResetCameraKey { get; private set; }

        // zoom in
        public Buttons ZoomInButton { get; private set; }
        public Keys ZoomInKey { get; private set; }

        // zoom out
        public Buttons ZoomOutButton { get; private set; }
        public Keys ZoomOutKey { get; private set; } 
        #endregion

        public Configuration()
        {
            ShowDebugView = true;

            // menu actions
            AcceptButton = Buttons.A;
            AcceptKey = Keys.Enter;

            BackButton = Buttons.B;
            BackKey = Keys.Back;

            
            // game actions
            DownButton = Buttons.LeftThumbstickDown;
            DownKey = Keys.Down;

            JumpButton = Buttons.A;
            JumpKey = Keys.Space;

            LeftButton = Buttons.LeftThumbstickLeft;
            LeftKey = Keys.Left;

            QuitButton = Buttons.Back;
            QuitKey = Keys.Escape;

            RightButton = Buttons.LeftThumbstickRight;
            RightKey = Keys.Right;

            UpButton = Buttons.LeftThumbstickUp;
            UpKey = Keys.Up;


            // camera actions
            ResetCameraButton = Buttons.RightStick;
            ResetCameraKey = Keys.R;

            ZoomInButton = Buttons.DPadUp;
            ZoomInKey = Keys.D1;

            ZoomOutButton = Buttons.DPadDown;
            ZoomOutKey = Keys.D2;
        }
    }
}
