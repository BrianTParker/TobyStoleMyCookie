using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAClient
{
    class Server
    {
        NetPeerConfiguration config;

        

        int numPlayers;

        // create and start server
        NetServer server;
        List<Player> playerList;

        // schedule initial sending of position updates
        double nextSendUpdates;

        //Other stuff server needs to keep track of
        Random r = new Random();
        int currentLevel;
        int previousLevel;
        List<Object> cookies;
        int lastCookieCount;
        
        

        

        public Server(){

            numPlayers = 0;
            currentLevel = 1;
            previousLevel = 0;
            //server stuff
            config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

            server = new NetServer(config);
            server.Start();

            nextSendUpdates = NetTime.Now;
            cookies = new List<Object>();
            playerList = new List<Player>();
            lastCookieCount = 0;

            
            

            
        }

        public Server(int inPort){

            numPlayers = 0;
            currentLevel = 1;

            //server stuff
            config = new NetPeerConfiguration("xnaapp");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = inPort;

            server = new NetServer(config);
            server.Start();

            nextSendUpdates = NetTime.Now;
            
        }

        public void launchServer()
        {

            //populate cookies
            
            
            // run until escape is pressed
            while (true)
            {

                //Console.WriteLine(cookies.Count);
                if (currentLevel != previousLevel)
                {
                    previousLevel = currentLevel;
                    populateCookies();

                    foreach (NetConnection player in server.Connections)
                    {
                        foreach (var c in cookies)
                        {

                            NetOutgoingMessage om = server.CreateMessage();
                            om.Write(c.getPos().X);
                            om.Write(c.getPos().Y);
                            om.Write(c.getId());
                            server.SendMessage(om, player, NetDeliveryMethod.UnreliableSequenced, 1);
                        }
                    }
                }

                if (cookies.Count == 0 && previousLevel != 0)
                {
                    currentLevel++;
                    foreach (NetConnection player in server.Connections)
                    {
                        NetOutgoingMessage om = server.CreateMessage();
                        om.Write(currentLevel);
                        server.SendMessage(om, player, NetDeliveryMethod.UnreliableSequenced, 4);
                    }
                }

                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            //
                            // Server received a discovery request from a client; send a discovery response (with no extra data attached)
                            //
                            server.SendDiscoveryResponse(null, msg.SenderEndPoint);

                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            //Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                //
                                // A new player just connected!
                                //
                                //Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                                Player newPlayer = new Player(msg.SenderConnection.RemoteUniqueIdentifier);
                                playerList.Add(newPlayer);
                                numPlayers += 1;
                                // randomize his position and store in connection tag
                                if (numPlayers == 1)
                                {
                                    msg.SenderConnection.Tag = new float[] { 150, 100 };
                                    //populateCookies();
                                    foreach (var c in cookies)
                                    {
                                        NetOutgoingMessage om = server.CreateMessage();

                                        om.Write(c.getPos().X);
                                        om.Write(c.getPos().Y);
                                        om.Write(c.getId());
                                        server.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.UnreliableSequenced, 1);
                                    }
                                    
                                }
                                else if (numPlayers == 3)
                                {
                                    msg.SenderConnection.Tag = new float[] { 950, 100 };
                                    Console.WriteLine("Should be sending " + cookies.Count + " cookies.");
                                    foreach (var c in cookies)
                                    {
                                        NetOutgoingMessage om = server.CreateMessage();
                                        
                                        om.Write(c.getPos().X);
                                        om.Write(c.getPos().Y);
                                        om.Write(c.getId());
                                        server.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.UnreliableSequenced, 1);
                                    }
                                }
                                else
                                {
                                    msg.SenderConnection.Tag = new float[] { 550, 100 };
                                    
                                    foreach (var c in cookies)
                                    {
                                        
                                        NetOutgoingMessage om = server.CreateMessage();
                                        om.Write(c.getPos().X);
                                        om.Write(c.getPos().Y);
                                        om.Write(c.getId());
                                        server.SendMessage(om, msg.SenderConnection, NetDeliveryMethod.UnreliableSequenced, 1);
                                    }
                                }
                                //msg.SenderConnection.Tag = new float[] {
                                //    NetRandom.Instance.Next(10, 100),
                                //    NetRandom.Instance.Next(10, 100)
                                //};
                                
                            }

                            break;
                        case NetIncomingMessageType.Data:
                            //
                            // The client sent input to the server
                            //
                            if (msg.SequenceChannel == 0)
                            {
                                float xinput = msg.ReadFloat();
                                float yinput = msg.ReadFloat();

                                float[] pos = msg.SenderConnection.Tag as float[];

                                // fancy movement logic goes here; we just append input to position
                                pos[0] += xinput;
                                pos[1] += yinput;
                            }
                            else if (msg.SequenceChannel == 1)
                            {
                                long playerId = msg.ReadInt64();
                                int cookieId = msg.ReadInt32();

                                foreach (var p in playerList)
                                {
                                    if (p.getId() == playerId)
                                    {
                                        p.increaseScore(1);
                                    }
                                    
                                }
                                List<Object> newCookies = new List<Object>(cookies);
                                int count = 0;
                                foreach (var c in newCookies)
                                {
                                    if (c.getId() == cookieId)
                                    {
                                        cookies.RemoveAt(count);
                                    }
                                    count++;
                                }

                                foreach (NetConnection player in server.Connections)
                                {
                                    if (player.RemoteUniqueIdentifier != playerId)
                                    {
                                        NetOutgoingMessage om = server.CreateMessage();
                                        om.Write(cookieId);
                                        server.SendMessage(om, player, NetDeliveryMethod.UnreliableSequenced, 2);
                                    }
                                }
                            }

                            //Console.WriteLine(pos[0]);
                            //Console.WriteLine(pos[1]);
                            break;
                    }

                    //
                    // send position updates 30 times per second
                    //
                    double now = NetTime.Now;


                    if (now > nextSendUpdates)
                    {
                        // Yes, it's time to send position updates

                        
                        // for each player...
                        foreach (NetConnection player in server.Connections)
                        {
                            // ... send information about every other player (actually including self)
                            foreach (NetConnection otherPlayer in server.Connections)
                            {
                                // send position update about 'otherPlayer' to 'player'
                                NetOutgoingMessage om = server.CreateMessage();

                                // write who this position is for
                                om.Write(otherPlayer.RemoteUniqueIdentifier);

                                if (otherPlayer.Tag == null)
                                    otherPlayer.Tag = new float[2];
                                //Console.WriteLine(otherPlayer.Tag);
                                float[] pos = otherPlayer.Tag as float[];
                                om.Write(pos[0]);
                                om.Write(pos[1]);

                                // send message
                                server.SendMessage(om, player, NetDeliveryMethod.UnreliableSequenced, 0);
                            }

                            //send information about the cookies
                            
                            
                        }

                        // schedule next update
                        nextSendUpdates += (1.0 / 30.0);
                    }
                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            server.Shutdown("app exiting");

        }

        public void populateCookies()
        {

            Random r = new Random();
            int id = 1;
            bool validPlacement = false;
            bool intersect = false;
            int tries = 0;
            int numCookies = 0;

            if (currentLevel == 1)
            {
                numCookies = 10;
            }
            else if (currentLevel == 2)
            {
                numCookies = 15;
            }
            else if (currentLevel == 3)
            {
                numCookies = 20;
            }
            else if (currentLevel == 4)
            {
                numCookies = 25;
            }

            for (int i = 0; i < numCookies; i++)
            {
                int x = r.Next(10, 1100);
                int y = r.Next(200, 400);
                if (cookies.Count > 0)
                {
                    while (validPlacement == false && tries < 10)
                    {
                        x = r.Next(10, 1100);
                        y = r.Next(200, 400);
                        foreach (var c in cookies)
                        {
                            Rectangle recA = new Rectangle((int)c.getPos().X, (int)c.getPos().Y, 60, 60);
                            Rectangle recB = new Rectangle(x, y, 60, 60);

                            if (recA.Intersects(recB))
                            {
                                intersect = true;
                                tries++;

                            }

                        }
                        if (intersect)
                        {
                            validPlacement = false;
                            intersect = false;

                        }
                        else
                        {
                            validPlacement = true;
                        }
                        
                    }

                    cookies.Add(new Object((float)x, (float)y, id));
                    validPlacement = false;
                    tries = 0;

                }
                else
                {
                    cookies.Add(new Object((float)x, (float)y, id));
                }





                id += 1;
            }
            
            
            
            
            
            
            //int id = 1;
            //Random r = new Random();

            //if (currentLevel == 1)
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        int x = r.Next(10, 1100);
            //        int y = r.Next(200, 400);
                    

            //        cookies.Add(new Object((float)x, (float)y, id));
            //        id += 1;
            //    }


            //}
            //else if (currentLevel == 2)
            //{
            //    for (int i = 0; i < 20; i++)
            //    {
            //        int x = r.Next(10, 1100);
            //        int y = r.Next(200, 400);


            //        cookies.Add(new Object((float)x, (float)y, id));
            //        id += 1;
            //    }
            //}


            
        }
        
    }
}
