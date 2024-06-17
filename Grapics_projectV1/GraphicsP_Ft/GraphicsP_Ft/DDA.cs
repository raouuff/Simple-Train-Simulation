using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphicsP_Ft
{
    public class DDA
    {
        public float X, Y;
        public float Xe, Ye;
        float dy, dx, m;
        public float cx, cy;
        int speed = 10;
        public void calc()
        {
            dy = Ye - Y;
            dx = Xe - X;
            m = dy / dx;
            cx = X;
            cy = Y;
        }
        public bool CalcNextPoint()
        {
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (X < Xe)
                {
                    cx += speed;
                    cy += m * speed;
                    if (cx >= Xe)
                    {
                        return false;
                    }

                }
                else
                {
                    cx -= speed;
                    cy -= m * speed;
                    if (cx <= Xe)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Y < Ye)
                {
                    cy += speed;
                    cx += 1 / m * speed;
                    if (cy >= Ye)
                    {
                        return false;
                    }
                }
                else
                {
                    cy -= speed;
                    cx -= 1 / m * speed;
                    if (cy <= Ye)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

    }
}
