using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAClient
{
    class Object
    {

        private Texture2D image;
        private float posx, posy;
        private Vector2 position;
        int id;

        public Object()
        {
            posx = 0;
            posy = 0;
            id = 0;
        }

        public Object(Texture2D inImage, float inX, float inY, int inId)
        {
            image = inImage;
            posx = inX;
            posy = inY;
            id = inId;
        }

        public Object(Texture2D inImage, float inX, float inY)
        {
            image = inImage;
            posx = inX;
            posy = inY;
        }

        public Object(float inX, float inY, int inId)
        {
            posx = inX;
            posy = inY;
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
            position = new Vector2(posx, posy);
            return position;
        }

        public int getId()
        {
            return id;
        }

        


    }
}
