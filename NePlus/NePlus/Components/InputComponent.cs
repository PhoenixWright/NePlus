using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using NePlus.Global;

namespace NePlus.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputComponent : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// represents key states that are useful to know
        /// </summary>
        public enum KeyState
        {
            Null,    // null value
            Up,      // key is still up this frame
            Down,    // key is still down this frame
            WentUp,  // key went up this frame
            WentDown // key went down this frame
        };

        public List<Dictionary<GameActions.Action, KeyState>> Input { get; private set; }

        public InputComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Input = new List<Dictionary<GameActions.Action, KeyState>>();
            UpdateInput();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // update the input
            UpdateInput();
            
            base.Update(gameTime);
        }
        
        public KeyState? GetKeyStateFromAction(GameActions.Action action)
        {
            if (Input.Count != 0)
            {
                try
                {
                    return Input[Input.Count - 1][action];
                }
                catch
                {
                    // it's okay if we don't find the action
                }
            }

            return null;
        }
        
        /// <summary>
        /// update the input structure
        /// </summary>
        private void UpdateInput()
        {
            // create a new dictionary to put all the keystates in
            Dictionary<GameActions.Action, KeyState> currentState = new Dictionary<GameActions.Action, KeyState>();

            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // TODO: add a configuration map to populate the dictionary with instead of hard coding controller controls here
            // eg: currentState.Add(GetActionFromKeyOrButton(keyOrButton), keyState - enum);

            if (currentGamePadState.IsConnected && currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                currentState.Add(GameActions.Action.Exit, KeyState.Down);
            }

            Input.Add(currentState);
            
            // empty some of the input out so it doesn't get too big
            if (Input.Count > 10)
            {
                Input.RemoveAt(0);
            }
        }
    }
}
