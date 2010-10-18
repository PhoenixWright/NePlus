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

namespace NePlus.Global
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Input : Microsoft.Xna.Framework.GameComponent
    {
        private List<Dictionary<Enums.Action, Enums.KeyState>> inputCollection { get; set; }

        public Input(Game game)
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
            inputCollection = new List<Dictionary<Enums.Action, Enums.KeyState>>();
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
        
        public Enums.KeyState? GetKeyStateFromAction(Enums.Action action)
        {
            if (inputCollection.Count != 0)
            {
                try
                {
                    return inputCollection[inputCollection.Count - 1][action];
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
            Dictionary<Enums.Action, Enums.KeyState> currentState = new Dictionary<Enums.Action, Enums.KeyState>();

            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // TODO: add a configuration map to populate the dictionary with instead of hard coding controller controls here, also see if there's a way to be generic about looping through the buttons
            // eg: currentState.Add(GetActionFromKeyOrButton(keyOrButton), keyState - enum);
            if (currentGamePadState.IsConnected && currentGamePadState.Buttons.A == ButtonState.Pressed)
            {
                currentState.Add(Enums.Action.JumpOrAccept, Enums.KeyState.Pressed);
            }
            
            if (currentGamePadState.IsConnected && currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                currentState.Add(Enums.Action.Exit, Enums.KeyState.Pressed);
            }
            
            if (currentGamePadState.IsConnected && currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
            {
                currentState.Add(Enums.Action.ResetCamera, Enums.KeyState.Pressed);
            }

            if (currentGamePadState.IsConnected && currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                currentState.Add(Enums.Action.ZoomIn, Enums.KeyState.Pressed);
            }

            if (currentGamePadState.IsConnected && currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                currentState.Add(Enums.Action.ZoomOut, Enums.KeyState.Pressed);
            }
           

            inputCollection.Add(currentState);
            
            // empty some of the input out so it doesn't get too big
            if (inputCollection.Count > 20)
            {
                inputCollection.RemoveAt(0);
            }
        }
    }
}
