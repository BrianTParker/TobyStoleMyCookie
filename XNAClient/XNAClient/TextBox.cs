using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAClient
{
    class TextBox
    {
        private static Dictionary<Keys, char> characterByKey;
        public StringBuilder Text;
        public Vector2 Position;
        public Color ForegroundColor;
        public Color BackgroundColor;
        public bool HasFocus;
        GraphicsDevice graphicsDevice;
        SpriteFont font;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;
        KeyboardState lastKeyboard;
        bool renderIsDirty = true;
        bool showing = true;

        static TextBox()
        {
            characterByKey = new Dictionary<Keys, char>()
            {
                //{Keys.A, 'a'},
                //{Keys.B, 'b'},
                //{Keys.C, 'c'},
                //{Keys.D, 'd'},
                //{Keys.E, 'e'},
                //{Keys.F, 'f'},
                //{Keys.G, 'g'},
                //{Keys.H, 'h'},
                //{Keys.I, 'i'},
                //{Keys.J, 'j'},
                //{Keys.K, 'k'},
                //{Keys.L, 'l'},
                //{Keys.M, 'm'},
                //{Keys.N, 'n'},
                //{Keys.O, 'o'},
                //{Keys.P, 'p'},
                //{Keys.Q, 'q'},
                //{Keys.R, 'r'},
                //{Keys.S, 's'},
                //{Keys.T, 't'},
                //{Keys.U, 'u'},
                //{Keys.V, 'v'},
                //{Keys.W, 'w'},
                //{Keys.X, 'x'},
                //{Keys.Y, 'y'},
                //{Keys.Z, 'z'},
                {Keys.D0, '0'},
                {Keys.D1, '1'},
                {Keys.D2, '2'},
                {Keys.D3, '3'},
                {Keys.D4, '4'},
                {Keys.D5, '5'},
                {Keys.D6, '6'},
                {Keys.D7, '7'},
                {Keys.D8, '8'},
                {Keys.D9, '9'},
                {Keys.NumPad0, '0'},
                {Keys.NumPad1, '1'},
                {Keys.NumPad2, '2'},
                {Keys.NumPad3, '3'},
                {Keys.NumPad4, '4'},
                {Keys.NumPad5, '5'},
                {Keys.NumPad6, '6'},
                {Keys.NumPad7, '7'},
                {Keys.NumPad8, '8'},
                {Keys.NumPad9, '9'},
                {Keys.OemPeriod, '.'}
            //    {Keys.OemMinus, '-'},
            //{Keys.Space, ' '}
            };

            
        }
        public TextBox(GraphicsDevice graphicsDevice, int width, SpriteFont font)
        {
            this.font = font;
            var fontMeasurements = font.MeasureString("dfgjlJL");
            var height = (int)fontMeasurements.Y;
            var pp = graphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphicsDevice,
            width,
            height,
            false, pp.BackBufferFormat, pp.DepthStencilFormat);
            Text = new StringBuilder();
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
        }


        public void Update(GameTime gameTime)
        {
            if (!HasFocus)
            {
                return;
            }
            var keyboard = Keyboard.GetState();
            foreach (var key in keyboard.GetPressedKeys())
            {
                if (!lastKeyboard.IsKeyUp(key))
                {
                    continue;
                }

                if (key == Keys.Delete || key == Keys.Back)
                {
                    if (Text.Length == 0)
                    {
                        continue;
                    }
                    Text.Length--;
                    renderIsDirty = true;
                    continue;
                }

                if (key == Keys.Enter)
                {
                    
                    showing = false;
                }

                char character;
                if (!characterByKey.TryGetValue(key, out character))
                {
                    continue;
                }
                if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
                {
                    character = Char.ToUpper(character);
                }
                
                Text.Append(character);
                renderIsDirty = true;
           }

                lastKeyboard = keyboard;
        }


        public void PreDraw()
        {
            if (!renderIsDirty)
            {
                return;
            }
            renderIsDirty = false;
            var existingRenderTargets = graphicsDevice.GetRenderTargets();
            graphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin();
            graphicsDevice.Clear(BackgroundColor);
            spriteBatch.DrawString(
                font, Text,
                Vector2.Zero, ForegroundColor);
            spriteBatch.End();
            graphicsDevice.SetRenderTargets(existingRenderTargets);
        }

        public void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget, Position, Color.White);
            spriteBatch.End();
        }

        public bool getShowing()
        {
            return showing;
        }

        public string getText()
        {
            return Text.ToString();
        }




    }
}
