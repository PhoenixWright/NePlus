using Microsoft.Xna.Framework.Input;

namespace NePlus.EngineComponents
{
    public class Configuration
    {
        /* Menu Actions */
        // accept (confirm)
        public Buttons AcceptButton { get; private set; }
        public Keys AcceptKey { get; private set; }

        // back (reject/deny)
        public Buttons BackButton { get; private set; }
        public Keys BackKey { get; private set; }
        

        /* Game Actions */
        // jump
        public Buttons JumpButton { get; private set; }
        public Keys JumpKey { get; private set; }

        // quit
        public Buttons QuitButton { get; private set; }
        public Keys QuitKey { get; private set; }

        // reset camera
        public Buttons ResetCameraButton { get; private set; }
        public Keys ResetCameraKey { get; private set; }

        // zoom in
        public Buttons ZoomInButton { get; private set; }
        public Keys ZoomInKey { get; private set; }

        // zoom out
        public Buttons ZoomOutButton { get; private set; }
        public Keys ZoomOutKey { get; private set; }

        public Configuration()
        {
            // menu actions
            AcceptButton = Buttons.A;
            AcceptKey = Keys.Enter;

            BackButton = Buttons.B;
            BackKey = Keys.Back;

            // game actions
            JumpButton = Buttons.A;
            JumpKey = Keys.Space;

            QuitButton = Buttons.Back;
            QuitKey = Keys.Escape;

            ResetCameraButton = Buttons.RightStick;
            ResetCameraKey = Keys.R;

            ZoomInButton = Buttons.DPadUp;
            ZoomInKey = Keys.D1;

            ZoomOutButton = Buttons.DPadDown;
            ZoomOutKey = Keys.D2;
        }
    }
}
