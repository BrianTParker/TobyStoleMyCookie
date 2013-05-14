using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAClient
{
    class Button
    {


        private bool visible;
        Texture2D image;
        int x, y;
        Vector2 pos;

        public Button(Texture2D inImage, int inX, int inY)
        {
            image = inImage;
            x = inX;
            y = inY;
            visible = false;
            pos = new Vector2(inX, inY);
        }

        public void makeVisible()
        {
            visible = true;
        }

        public void makeInvisible()
        {
            visible = false;
        }

        public bool isVisible()
        {
            return visible;
        }

        public Vector2 getPos()
        {
            return pos;
        }

        public Texture2D getImage()
        {

            return image;
        }


    }
}
