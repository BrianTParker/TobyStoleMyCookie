using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAClient
{
    class CollisionDetection
    {

        public bool PerPixel(Rectangle RectangleA, Texture2D SpriteA, Rectangle RectangleB, Texture2D SpriteB)
        {
            Color[] DataA = new Color[SpriteA.Width * SpriteA.Height];
            SpriteA.GetData(DataA);
            Color[] DataB = new Color[SpriteB.Width * SpriteB.Height];
            SpriteB.GetData(DataB);
            int Top = System.Math.Max(RectangleA.Top, RectangleB.Top);
            int Bottom = System.Math.Min(RectangleA.Bottom, RectangleB.Bottom);
            int Left = System.Math.Max(RectangleA.Left, RectangleB.Left);
            int Right = System.Math.Min(RectangleA.Right, RectangleB.Right);
            for (int y = Top; y < Bottom; y++)
            {
                for (int x = Left; x < Right; x++)
                {
                    Color ColorA = DataA[(x - RectangleA.Left) + (y - RectangleA.Top) * RectangleA.Width];
                    Color ColorB = DataB[(x - RectangleB.Left) + (y - RectangleB.Top) * RectangleB.Width];
                    if (ColorA.A > 30 && ColorB.A > 30)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool Rectangular(Rectangle RectangleA, Rectangle RectangleB)
        {
            return RectangleA.Intersects(RectangleB);
        }
    }
    
}


