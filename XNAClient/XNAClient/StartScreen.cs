//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace XNAClient
//{
//    class StartScreen
//    {

//        private Texture2D texture;
//        private Game1 game;
//        private KeyboardState lastState;

//        public StartScreen(Game1 game)
//        {
//            this.game = game;
//            texture = game.Content.Load<Texture2D>("start_screen");
//            lastState = Keyboard.GetState();
//        }

//        public void Update()
//        {
//            KeyboardState keyboardState = Keyboard.GetState();

//            if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
//            {
//                game.StartGame();
//            }

//            lastState = keyboardState;
//        }

//        public void Draw(SpriteBatch spriteBatch)
//        {
//            if (texture != null)
//                spriteBatch.Draw(texture, new Vector2(0f, 0f), Color.White);
//        }
//    }
//}
