using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;


using Lidgren.Network;
using System.Collections;
using System.ComponentModel;
using System.Threading;

namespace XNAClient
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
       
        private Texture2D cookieImage;
        private Texture2D tobyLeft1;
        private Texture2D tobyLeft2;
        private Texture2D tobyRight1;
        private Texture2D tobyRight2;
        private Texture2D tobyStanding;
        private Texture2D trampolineImage1;
        private Texture2D trampolineImage2;
        private Texture2D background;
        private Texture2D truettJump;
        private Texture2D truettFall;
        private Texture2D single_player_button;
        private Texture2D multi_player_button;
        private Texture2D host_button;
        private Texture2D join_button;
        private Texture2D internet_button;
        private Texture2D lan_button;
        private Texture2D start_button;

        CollisionDetection collision = new CollisionDetection();

        //Game states
        public enum GameState
        {
            mainMenu,
            multiplayerMenu,
            hostOptions,
            joinOptions,
            enterIp,
            playing,
            lobby
        }

        GameState state;

        

        List<Player> allPlayers = new List<Player>();
        List<Object> cookies = new List<Object>();
        Player player;
        Player toby;
        Object[] trampolines;
        Dictionary<long, Vector2> positions = new Dictionary<long, Vector2>();
        NetClient client;
        bool firstPass = true;
        int walk_count = 0;
        TextBox textbox;

        string remoteIp;

        private delegate void SomeFunctionDelegate();
        private SomeFunctionDelegate sfd;

        Server server;

        Button singlePlayerButton;
        Button multiplayerButton;
        Button joinGame;
        Button hostGame;
        Button joinLan;
        Button joinIneternet;
        Button hostLan;
        Button hostInternet;
        Button startGame;

        string gameType;

        MouseState mouseState;
        Vector2 mousePos;

        bool LAN = false;
        bool host = false;
        
        int localLevel;
        int remoteLevel;

        int numPlayers;
        


        public Game1()
        {
            
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
            
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            
            client = new NetClient(config);
            client.Start();
            
            
            state = GameState.mainMenu;
            int localLevel = 1;
            int remoteLevel = 1;
            numPlayers = 1;
        }

        protected override void Initialize()
        {
            
            this.IsMouseVisible = true; 
            base.Initialize();

            

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            trampolines = new Object[3];
            cookieImage = Content.Load<Texture2D>("cookie");
            tobyLeft1 = Content.Load<Texture2D>("toby_left1");
            tobyLeft2 = Content.Load<Texture2D>("toby_left2");
            tobyRight1 = Content.Load<Texture2D>("toby_right1");
            tobyRight2 = Content.Load<Texture2D>("toby_right2");
            tobyStanding = Content.Load<Texture2D>("toby_stand");
            trampolineImage1 = Content.Load<Texture2D>("trampoline");
            trampolineImage2 = Content.Load<Texture2D>("trampoline_pressed");
            truettFall = Content.Load<Texture2D>("truett");
            truettJump = Content.Load<Texture2D>("truett_jump");
            background = Content.Load<Texture2D>("park");
            single_player_button = Content.Load<Texture2D>("single_player");
            multi_player_button = Content.Load<Texture2D>("multi_player");
            host_button = Content.Load<Texture2D>("host");
            join_button = Content.Load<Texture2D>("join");
            internet_button = Content.Load<Texture2D>("internet");
            lan_button = Content.Load<Texture2D>("lan");
            start_button = Content.Load<Texture2D>("startGame");



            textbox = new TextBox(GraphicsDevice, 400, Content.Load<SpriteFont>("Text"))
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.CornflowerBlue,
                Position = new Vector2(100, 300),
                HasFocus = true

            };

            float xPosition = 150;
            for (int i = 0; i < 3; i++)
            {
                trampolines[i] = new Object(trampolineImage1, xPosition, 500);
                xPosition += 400;
            }
            Console.WriteLine(client.UniqueIdentifier);
            player = new Player(truettFall, 150, 100, client.UniqueIdentifier);
            toby = new Player(tobyRight1, 50, 550, -1);
            //allPlayers.Add(player);

            singlePlayerButton = new Button(single_player_button, 100, 100);
            multiplayerButton = new Button(multi_player_button, 100, 200);
            joinGame = new Button(join_button, 300, 100);
            hostGame = new Button(host_button, 300, 200);
            joinLan = new Button(lan_button, 100, 100);
            joinIneternet = new Button(internet_button, 100, 200);
            hostLan = new Button(lan_button, 100, 100);
            hostInternet = new Button(internet_button, 100, 200);
            startGame = new Button(start_button, 300, 100);



                
        }

        protected override void Update(GameTime gameTime)
        {
            //
            // Collect input
            //
            
                float xinput = 0;
                float yinput = 0;

                

                if (state != GameState.playing)
                {
                    mouseState = Mouse.GetState();
                




                


                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (singlePlayerButton.isVisible())
                        {
                            Rectangle recA = new Rectangle((int)singlePlayerButton.getPos().X, (int)singlePlayerButton.getPos().Y, singlePlayerButton.getImage().Width, singlePlayerButton.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {
                                gameType = "single";

                                server = new Server();
                                sfd = new SomeFunctionDelegate(server.launchServer);
                                sfd.BeginInvoke(null, null);
                                //host++;
                                LAN = true;
                                client.DiscoverLocalPeers(14242);
                            
                                state = GameState.playing;


                            }
                        }
                        if (multiplayerButton.isVisible())
                        {
                            Rectangle recA = new Rectangle((int)multiplayerButton.getPos().X, (int)multiplayerButton.getPos().Y, multiplayerButton.getImage().Width, multiplayerButton.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {

                                state = GameState.multiplayerMenu;



                            }
                        }
                        if (hostGame.isVisible())
                        {
                            Rectangle recA = new Rectangle((int)hostGame.getPos().X, (int)hostGame.getPos().Y, hostGame.getImage().Width, hostGame.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {


                                gameType = "multi";
                                server = new Server();
                                sfd = new SomeFunctionDelegate(server.launchServer);
                                sfd.BeginInvoke(null, null);
                                host = true;
                                LAN = true;
                                client.DiscoverLocalPeers(14242);
                            

                            

                                state = GameState.lobby;
                            }
                        }
                        if (joinGame.isVisible())
                        {
                            Rectangle recA = new Rectangle((int)joinGame.getPos().X, (int)joinGame.getPos().Y, joinGame.getImage().Width, joinGame.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {
                                state = GameState.joinOptions;
                            }
                        }
                        if (joinIneternet.isVisible())
                        {
                            Rectangle recA = new Rectangle((int)joinIneternet.getPos().X, (int)joinIneternet.getPos().Y, joinIneternet.getImage().Width, joinIneternet.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {
                                state = GameState.enterIp;
                                gameType = "multi";
                            }
                        }
                        if (joinLan.isVisible())
                        {
                            Rectangle recA = new Rectangle((int)joinLan.getPos().X, (int)joinLan.getPos().Y, joinLan.getImage().Width, joinLan.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {
                                client.DiscoverLocalPeers(14242);
                                state = GameState.lobby;
                                gameType = "multi";
                                LAN = true;
                            }
                        }
                        if (startGame.isVisible() && host == true)
                        {
                            Rectangle recA = new Rectangle((int)startGame.getPos().X, (int)startGame.getPos().Y, startGame.getImage().Width, startGame.getImage().Height);
                            Rectangle recB = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
                            if (recA.Contains(recB))
                            {
                                state = GameState.playing;
                                NetOutgoingMessage om = client.CreateMessage();
                                om.Write(player.getId());
                                om.Write(1);
                                client.SendMessage(om, NetDeliveryMethod.UnreliableSequenced, 6);
                                
                            }
                        }
                    }

                    if (state == GameState.lobby)
                    {
                        //Console.WriteLine(allPlayers.Count);
                        NetIncomingMessage msg;
                        while ((msg = client.ReadMessage()) != null)
                        {
                            bool playerFound = false;
                            bool cookieFound = false;
                            switch (msg.MessageType)
                            {
                                case NetIncomingMessageType.DiscoveryResponse:
                                    // just connect to first server discovered
                                   // Console.WriteLine("Received a request response");
                                    if (LAN)
                                    {
                                        client.Connect(msg.SenderEndPoint);
                                    }

                                    break;
                                case NetIncomingMessageType.Data:

                                    if (msg.SequenceChannel == 0)
                                    {
                                        // server sent a position update
                                        long who = msg.ReadInt64();
                                        float x = msg.ReadFloat();
                                        float y = msg.ReadFloat();
                                        int playerNum = msg.ReadInt32();


                                        Player newPlayer = null;
                                        foreach (var p in allPlayers)
                                        {                                 



                                            if (who == p.getId())
                                            {
                                                if (p.getId() == player.getId())
                                                {
                                                    if (firstPass == true && x > 0)
                                                    {
                                                        player.newPos(new Vector2(x, y));

                                                        firstPass = false;
                                                    }


                                                    if (player.getPlayerNum() != playerNum)
                                                    {

                                                        player.setPlayerNum(playerNum);
                                                    }


                                                }
                                                

                                                playerFound = true;
                                            }
                                            
                                            

                                        }
                                        if (playerFound == false)
                                        {
                                            newPlayer = new Player(truettFall, x, y, who, numPlayers);
                                            allPlayers.Add(newPlayer);
                                            numPlayers++;
                                        }

                                        positions[who] = new Vector2(x, y);

                                    }
                                    else if (msg.SequenceChannel == 1)
                                    {

                                        //cookies.Clear();
                                        float x = msg.ReadFloat();
                                        float y = msg.ReadFloat();
                                        int id = msg.ReadInt32();


                                        foreach (var c in cookies)
                                        {
                                            if (c.getId() == id)
                                            {
                                                cookieFound = true;
                                            }


                                        }
                                        if (cookieFound == false)
                                        {
                                            Object newCookie = new Object(cookieImage, x, y, id);
                                            cookies.Add(newCookie);
                                        }
                                    }
                                    else if (msg.SequenceChannel == 6)
                                    {
                                        int start = msg.ReadInt32();
                                        if (start == 1)
                                        {
                                            state = GameState.playing;
                                        }
                                    }

                                    break;
                            }
                        }

                        
                    }
                      
                }
                else
                {
                    KeyboardState keyState = Keyboard.GetState();

                // exit game if escape or Back is pressed
                if (keyState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // use arrows or dpad to move avatar
                if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Left))
                    if (player.getPos().X > 0)
                    {
                        xinput = -3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Right))
                    if (player.getPos().X < 1160)
                    {
                        xinput = 3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                //if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))

                //if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))


                if (player.getPos().Y < 480 && player.goingUp() == false)
                {
                    yinput = 3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    foreach (var tramp in trampolines)
                    {

                        Rectangle recA = new Rectangle((int)player.getPos().X, (int)player.getPos().Y, player.getImage().Width, player.getImage().Height);
                        Rectangle recB = new Rectangle((int)tramp.getPos().X, (int)tramp.getPos().Y, tramp.getImage().Width, tramp.getImage().Height);
                        if (collision.PerPixel(recA, player.getImage(), recB, tramp.getImage()) && (player.getPos().Y + 100 <= tramp.getPos().Y))
                        {
                            player.updateDirection();
                            //player.updateImage(truettJump);
                            break;
                        }


                    }








                    //yinput = 3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;

                }
                else if (player.getPos().Y >= 0 && player.goingUp() == true)
                {
                    yinput = -3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (player.getPos().Y <= 50)
                    {
                        player.updateDirection();
                        //player.updateImage(truettFall);
                    }
                }

                
                
                    

                for (int x = 0; x < cookies.Count; x++)
                {

                    Rectangle recA = new Rectangle((int)player.getPos().X, (int)player.getPos().Y, 114, 240);
                    Rectangle recB = new Rectangle((int)cookies[x].getPos().X, (int)cookies[x].getPos().Y, 60, 60);

                    if (recA.Intersects(recB))
                    {

                        NetOutgoingMessage om = client.CreateMessage();
                        om.Write(player.getId());
                        om.Write(cookies[x].getId());
                        client.SendMessage(om, NetDeliveryMethod.UnreliableSequenced, 1);

                        cookies.RemoveAt(x);
                        
                        //We just removed an object, so we set x back by one to make sure we skip none
                        //Just sit down and think about it, it makes sense
                        x--;


                    
                    
                        
                        

                    

                        
                            
                    }
                        


                    
                    


                }
                if (xinput != 0 || yinput != 0)
                {

                    player.updatePosX(xinput);
                    player.updatePosY(yinput);
                    //player.newPos(new Vector2(xinput, yinput));
                    //Console.WriteLine(player.getPos().X + " " + player.getPos().Y);
                    //
                    // If there's input; send it to server
                    //
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write(player.getId());
                    om.Write(xinput); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity
                    om.Write(yinput);
                    client.SendMessage(om, NetDeliveryMethod.UnreliableSequenced, 0);
                }

                //move all the other players
                foreach (var p in allPlayers)
                {
                    if (player.getId() != p.getId())
                    {
                        if (p.getMoveX() == 1)
                        {
                            p.updatePosX(3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        }
                        else if (p.getMoveX() == -1)
                        {
                            p.updatePosX(-3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        }

                        if (p.getMoveY() == 1)
                        {
                            p.updatePosY(3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            //p.updateImage(truettJump);
                        }
                        else if (p.getMoveY() == -1)
                        {
                            p.updatePosY(-3 * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            //p.updateImage(truettFall);
                        }
                    }

                }

                //move toby
                moveToby(gameTime);


                // read messages
                NetIncomingMessage msg;
                while ((msg = client.ReadMessage()) != null)
                {
                    bool playerFound = false;
                    bool cookieFound = false;
                    switch (msg.MessageType)
                    {
                        //case NetIncomingMessageType.DiscoveryResponse:
                        //    // just connect to first server discovered
                        //    Console.WriteLine("Received a request response");
                        //    if (LAN)
                        //    {
                        //        client.Connect(msg.SenderEndPoint);
                        //    }
                            
                        //    break;
                        case NetIncomingMessageType.Data:

                            if (msg.SequenceChannel == 0)
                            {
                                // server sent a position update
                                long who = msg.ReadInt64();
                                float x = msg.ReadFloat();
                                float y = msg.ReadFloat();
                                int playerNum = msg.ReadInt32();


                                
                                foreach (var p in allPlayers)
                                {



                                    if (who == p.getId())
                                    {
                                        if (p.getId() != player.getId())
                                        {
                                            float xDiff = Math.Abs(x - p.getPos().X);
                                            float yDiff = Math.Abs(y - p.getPos().Y);

                                            

                                            if (xDiff <= 5)
                                            {
                                                p.setMoveX(0);
                                            }
                                            else if (x > p.getPos().X)
                                            {
                                                p.setMoveX(1);
                                            }
                                            else if (x < p.getPos().X)
                                            {
                                                p.setMoveX(-1);
                                            }





                                            if (y == p.getPos().Y)
                                            {
                                                p.setMoveY(0);
                                            }
                                            else if (y > p.getPos().Y)
                                            {
                                                p.setMoveY(1);
                                            }
                                            else
                                            {
                                                p.setMoveY(-1);
                                            }

                                        }
                                    }
                                    

                                }
                                

                                positions[who] = new Vector2(x, y);

                            }
                            if (msg.SequenceChannel == 1)
                            {

                                //cookies.Clear();
                                float x = msg.ReadFloat();
                                float y = msg.ReadFloat();
                                int id = msg.ReadInt32();


                                foreach (var c in cookies)
                                {
                                    if (c.getId() == id)
                                    {
                                        cookieFound = true;
                                    }


                                }
                                if (cookieFound == false)
                                {
                                    Object newCookie = new Object(cookieImage, x, y, id);
                                    cookies.Add(newCookie);
                                }
                            }
                            else if (msg.SequenceChannel == 2)
                            {
                                int cookieId = msg.ReadInt32();
                                List<Object> deleteCookies = new List<Object>(cookies);

                                foreach (var c in deleteCookies)
                                {
                                    if (c.getId() == cookieId)
                                    {
                                        cookies.Remove(c);
                                    }


                                }
                            }
                            else if (msg.SequenceChannel == 4)
                            {
                                remoteLevel = msg.ReadInt32();
                                if (localLevel != remoteLevel)
                                {
                                    localLevel = remoteLevel;
                                    Thread.Sleep(2000);
                                }
                            }

                            break;
                    }
                }

                textbox.Update(gameTime);
                base.Update(gameTime);


            }
            
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //this.graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

            switch (state)
            {
                case GameState.mainMenu:
                    //reset all the other buttons to be invisible
                    hostGame.makeInvisible();
                    joinGame.makeInvisible();
                    joinIneternet.makeInvisible();
                    joinLan.makeInvisible();

                    singlePlayerButton.makeVisible();
                    multiplayerButton.makeVisible();
                    spriteBatch.Draw(singlePlayerButton.getImage(), singlePlayerButton.getPos(), Color.White);
                    spriteBatch.Draw(multiplayerButton.getImage(), multiplayerButton.getPos(), Color.White);
                    break;
                case GameState.multiplayerMenu:
                    singlePlayerButton.makeInvisible();
                    multiplayerButton.makeInvisible();
                    hostGame.makeVisible();
                    joinGame.makeVisible();
                    spriteBatch.Draw(hostGame.getImage(), hostGame.getPos(), Color.White);
                    spriteBatch.Draw(joinGame.getImage(), joinGame.getPos(), Color.White);

                    break;
                case GameState.joinOptions:
                    hostGame.makeInvisible();
                    joinGame.makeInvisible();
                    joinIneternet.makeVisible();
                    joinLan.makeVisible();
                    spriteBatch.Draw(joinIneternet.getImage(), joinIneternet.getPos(), Color.White);
                    spriteBatch.Draw(joinLan.getImage(), joinLan.getPos(), Color.White);
                    
                    break;
                case GameState.enterIp:
                    joinIneternet.makeInvisible();
                    joinLan.makeInvisible();

                    if (textbox.getShowing())
                    {
                        textbox.Update(gameTime);
                        textbox.PreDraw();
                        GraphicsDevice.Clear(Color.Black);
                        textbox.Draw();
                    }
                    else
                    {
                        remoteIp = textbox.getText();
                        client.Connect(remoteIp, 14242);
                        state = GameState.lobby;

                    }
                    

                    break;

                case GameState.lobby:


                    int stringY = 0;
                    foreach (var players in allPlayers)
                    {
                        stringY += 100;
                        spriteBatch.DrawString(Content.Load<SpriteFont>("Text"), "Player " + players.getPlayerNum(), new Vector2(30, stringY), Color.White);
                    }

                    if (allPlayers.Count > 1 && host == true)
                    {
                        startGame.makeVisible();
                        spriteBatch.Draw(startGame.getImage(), startGame.getPos(), Color.White);
                    }
                    break;

                case GameState.playing:
                    // draw all players
                    foreach (var kvp in allPlayers)
                    {
                        // use player unique identifier to choose an image
                        //int num = Math.Abs((int)kvp.Key) % textures.Length;


                        //draw cookies
                        foreach (var c in cookies)
                        {
                            spriteBatch.Draw(c.getImage(), c.getPos(), Color.White);
                        }

                        // draw player
                        if (kvp.getId() != player.getId())
                        {
                            spriteBatch.Draw(kvp.getImage(), kvp.getPos(), Color.White);

                        }
                        else
                        {
                            //Console.WriteLine(player.getPos().X + " " + player.getPos().Y);
                            spriteBatch.Draw(player.getImage(), player.getPos(), Color.White);
                        }

                        foreach (var tramps in trampolines)
                        {
                            spriteBatch.Draw(tramps.getImage(), tramps.getPos(), Color.White);
                        }

                        spriteBatch.Draw(toby.getImage(), toby.getPos(), Color.White);

                    }

                    break;

                

            }

            

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Shutdown("bye");

            base.OnExiting(sender, args);
        }

        public void moveToby(GameTime gameTime)
        {

            if (toby.getPos().X < 1200 && toby.getDirection() == "right")
            {
                walk_count += 1;

                toby.updatePosX(3);

                if (walk_count == 20)
                {
                    if (toby.getImage() == tobyRight1)
                    {
                        toby.updateImage(tobyRight2);

                    }
                    else
                    {
                        toby.updateImage(tobyRight1);
                    }
                    walk_count = 0;
                }

                if (toby.getPos().X >= 1100)
                {
                    toby.updateTobyDirection();
                    toby.updateImage(tobyLeft1);
                }
            }
            else
            {
                walk_count += 1;

                toby.updatePosX(-3);

                if (walk_count == 20)
                {
                    if (toby.getImage() == tobyLeft1)
                    {
                        toby.updateImage(tobyLeft2);
                    }
                    else
                    {
                        toby.updateImage(tobyLeft1);
                    }

                    walk_count = 0;
                }

                if (toby.getPos().X <= 10)
                {
                    toby.updateTobyDirection();
                    toby.updateImage(tobyRight1);
                }
                
                
            }

        }
    }

    

}
