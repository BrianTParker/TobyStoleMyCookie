using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAClient
{
    class Player
    {
        private long id;
        private Texture2D image;
        private float posx, posy;
        private Vector2 position;
        private bool up;
        int moveX, moveY;
        string direction;
        int score;

        public Player(Texture2D inImage, float inX, float inY, long inId)
        {
            image = inImage;
            posx = inX;
            posy = inY;
            id = inId;
            position = new Vector2(posx, posy);
            up = false;
            moveX = 0;
            moveY = 0;
            direction = "right";
            score = 0;
        }

        public Player(long inId)
        {
            score = 0;
            id = inId;
        }

        public void updateImage(Texture2D newImage)
        {
            image = newImage;
        }

        public Texture2D getImage()
        {
            return image;
        }

        public Vector2 getPos()
        {
          
            return position;
        }

        public Vector2 getIntPos()
        {
            Vector2 intPos = new Vector2((int)posx, (int)posy);
            return intPos;
        }

        public void updatePosX(float inX)
        {
            position.X += inX;
        }

        public void updatePosY(float inY)
        {
            position.Y += inY;
        }

        public long getId()
        {
            return id;
        }

        public void newPos(Vector2 inPos)
        {
            position = inPos;

        }

        public void updateDirection()
        {
            if (up == true)
            {
                up = false;
            }
            else
            {
                up = true;
            }
        }

        public bool goingUp()
        {
            return up;
        }

        public void updateMoveX()
        {
            if (moveX == 1)
            {
                moveX = -1;
            }
            else
            {
                moveX = 1;
            }
        }

        public void updateMoveY()
        {
            if (moveY == 1)
            {
                moveY = -1;
            }
            else
            {
                moveY = 1;
            }
        }

        public void setMoveX(int inX)
        {
            moveX = inX;
        }

        public void setMoveY(int inY)
        {
            moveY = inY;
        }

        public int getMoveX()
        {
            return moveX;
        }

        public int getMoveY()
        {
            return moveY;
        }

        public void updateTobyDirection()
        {
            if (direction == "right")
            {
                direction = "left";
            }
            else
            {
                direction = "right";
            }
        }

        public string getDirection()
        {
            return direction;
        }

        public int getScore()
        {
            return score;
        }

        public void increaseScore(int amount)
        {
            score += amount;
        }
    }
}
