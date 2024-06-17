using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsP_Ft
{

    public class Train
    {
        public float X;
        public float Y;
        public bool Mline = false;
        public bool Mcircle = false;
        public bool Mcurve;
        // Alf kam mara 3la kol circle 
        public int Rounds = 0;
        // catch el circle el ana 3ndha
        public int posC = 0;
        public int posV = 0;
    }
  

    public class Map
    {
        public Rectangle rcDst;
        public Rectangle rcSrc;
        public bool moveF = false;
        public bool moveB = false;
        public Bitmap img;
    }
    public partial class Form1 : Form
    {
        int ts = 1;
        List<DDA> lines = new List<DDA>();
        List<BezierCurve> curves = new List<BezierCurve>();
        List<Circle> circles = new List<Circle>();
        Bitmap off; 
        Bitmap img;
        bool animation = false;
        int pointerL = 0;
        Train train = new Train();
        float rotationAngle = 0.0f;
        string obj = "";
        List<string> Objects = new List<string>();
        Point ptstart, ptend;
        Timer tt = new Timer();
        List<Map> Lmap = new List<Map>();
        //to tell me the thing has been created end where 
        int pointerX = 0;
        float time = 0.0f;
        

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.KeyDown += Form1_KeyDown;
            tt.Tick += Tt_Tick;
            tt.Interval = 1;
            tt.Start();
        }
        private void Tt_Tick(object sender, EventArgs e)
        {
            if (animation)
            {
                CatchCircle();
                MOnCircle();
                MOnLine();
                CatchCurve();
                MOnCurve();

                // Scroll the map based on train position
                int screenWidth = this.ClientSize.Width;
                int quarterScreenWidth = screenWidth / 4;
                int halfScreenWidth = screenWidth / 2;
                int threeQuarterScreenWidth = screenWidth * 3 / 4;
                if(!train.Mcurve&&!train.Mcurve)
                {
                    if (train.X >= quarterScreenWidth && train.X < halfScreenWidth)
                    {
                        While_Scrolling(10, -1);

                    }
                    else if (train.X >= halfScreenWidth && train.X < threeQuarterScreenWidth)
                    {
                        While_Scrolling(10, -1);

                    }
                    else if (train.X >= threeQuarterScreenWidth && train.X <= screenWidth)
                    {
                        While_Scrolling(10, -1);

                    }
                }
                

                // Restart if the animation completes
                if (train.X + img.Width >= lines[lines.Count - 1].Xe)
                {
                    animation = false;
                    MessageBox.Show("Completed");
                    lines.Clear();
                    circles.Clear();
                    curves.Clear();
                    Objects.Clear();
                    obj = "";
                    pointerX = 0;
                    pointerL = 0;
                    train.posV = 0;
                    train.posC = 0;
                }
            }

            DrawDubb(this.CreateGraphics());
        }

        private void ScrollMapRight(int scrollSpeed)
        {
            foreach (var map in Lmap)
            {
                map.rcSrc.X += scrollSpeed;
                if (map.rcSrc.X >= map.img.Width)
                {
                    map.rcSrc.X = 0;
                }
            }
            While_Scrolling(scrollSpeed, -1);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if(e.KeyCode == Keys.A)
            {
                ts += 2;
            }
            //this part for creating objects
            if (e.KeyCode == Keys.D1&&!animation)
            {
                obj = "line";
                Objects.Add(obj);
                CreateLine();
            }
            else if (e.KeyCode == Keys.D2 && !animation)
            {
                obj = "circle";
                Objects.Add(obj);

                CreateCircle();
            }
            else if (e.KeyCode == Keys.D3 && !animation)
            {
                obj = "curve";
                Objects.Add(obj);

                CreateCurve();

            }
            else if (e.KeyCode == Keys.D && !animation)
            {
                DeleteLastObject();
            }
            ///////////////////////////////
            //this for modifing 0-> increament , 1->decreament
            if (e.KeyCode == Keys.Up && !animation)
            {
                if (obj == "line")
                {
                    ModifyLines(0);
                }
                else if (obj == "curve" && !animation)
                {
                    ModifyCurve(0);
                }
                else if (obj == "circle" && !animation)
                {
                    ModifyCircle(0);

                }
            }
            else if (e.KeyCode == Keys.Down && !animation)
            {
                if (obj == "line")
                {
                    ModifyLines(1);

                }
                else if (obj == "curve")
                {
                    ModifyCurve(1);
                }
                else if (obj == "circle")
                {
                    ModifyCircle(1);
                }
            }
            if (e.KeyCode == Keys.Enter&&!animation&&obj!="curve")
            {
                train.Mline = true;
                animation = true;
            }
            else if(e.KeyCode == Keys.Enter&&obj=="curve")
            {
                MessageBox.Show("Please add line to the end of the path for safety purposes");
            }
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if(!animation)
                    {
                        Lmap[0].rcSrc.X += 20;
                        While_Scrolling(20, -1);
                        if (Lmap[0].rcSrc.X >= Lmap[0].img.Width)
                        {
                            Lmap[0].rcSrc.X = 0;
                        }
                    }
                   
                    break;
                case Keys.Left:

                    if(!animation)
                    {
                        if (Lmap[0].rcSrc.X >= 10)
                        {
                            Lmap[0].rcSrc.X -= 20;
                            While_Scrolling(20, 1);
                        }
                    }
                   
                    break;
            }
            this.Text = obj;
            DrawDubb(this.CreateGraphics());
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(this.CreateGraphics());
        }
        void While_Scrolling(int speed , int d)
        {
            WhileMLine(speed,d);
            WhileMCircle(speed,d);
            WhileMCurve(speed,d);
            DDA ob = lines[lines.Count - 1];
            ptstart.X =(int) ob.X;
            ptend.X = (int)ob.Xe;
            //there was a bug that when the scrloing work it keep a gap between last object and the new 
            //updating pointerX
            switch(obj)
            {
                case "line":
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    break;
                case "curve":
                    pointerX = (int)curves[curves.Count - 1].ControlPoints[2].X;
                    break;
                case "circle":
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    break;
            }
            DrawDubb(this.CreateGraphics());

        }

        void WhileMLine(int speed, int d)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].X += speed * d;
                lines[i].Xe += speed * d;
                lines[i].cx += speed * d;
                
            }
        }
        void WhileMCircle(int speed, int d)
        {
            for (int i = 0; i < circles.Count; i++)
            {
                circles[i].XC += speed * d;

            }
        }
        void WhileMCurve(int speed, int d)
        {
            for (int i = 0; i < curves.Count; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    Point get = new Point();
                    get = curves[i].ControlPoints[k];
                    get.X += speed * d;
                    curves[i].ControlPoints[k] = get;
                }
            }
        }
      void MOnCurve()
{
    if (train.Mcurve)
    {
        PointF currentPoint = curves[train.posV].CalcCurvePointAtTime(time);
        PointF nextPoint = curves[train.posV].CalcCurvePointAtTime(time + 0.01f); // Estimate the next point

        // Calculate the angle between the current and next points
        double angle = Math.Atan2(nextPoint.Y - currentPoint.Y, nextPoint.X - currentPoint.X) * (180 / Math.PI);
        if (angle < 0) angle += 360; // Ensure angle is in the range [0, 360]

        train.Rounds = (int)angle; // Update the rotation angle

        // Move the train along the curve
        train.X = currentPoint.X * ts;
        train.Y = currentPoint.Y;
        time += 0.01f;

        // Check if the animation is complete
        if (time >= 1)
        {
            train.Mcurve = false;
            train.Mline = true;
            train.posV += 1;
            train.Rounds = 0;
            time = 0;
        }
    }
}

        //void MOnCurve()
        //{
        //    if (train.Mcurve)
        //    {
        //        PointF x;
        //        x = curves[train.posV].CalcCurvePointAtTime(time);
        //        train.X = x.X;
        //        train.Y = x.Y;
        //        time += 0.01f;
        //        if(time<0.49)
        //        {
        //            train.Rounds -= 5;
        //        }
        //        else if(time>=0.5)
        //        {

        //            train.Rounds+= 5;
        //        }
        //        if (time >= 1)
        //        {
        //            train.Mcurve = false;
        //            train.Mline = true;
        //            train.posV +=1 ;
        //            train.Rounds = 0;
        //            time = 0;
        //        }
        //    }

        //}
        void CatchCurve()
        {
            for (int i = train.posV; i < curves.Count; i++)
            {
                PointF x = new PointF();
                x = curves[i].CalcCurvePointAtTime(0);
                if (train.X >= x.X)
                {
                    train.Mcurve = true;
                    train.Mline = false;
                    train.posV = i;
                    break;
                }

                
            }
        }
        void MOnCircle()
        {
            if (train.Mcircle)
            {
                PointF pball = circles[train.posC].Getnextpoint(train.Rounds);
                // this.Text = c.thRadian.ToString();
                train.Rounds -= 5;
                train.X = pball.X*ts;
                train.Y = pball.Y;
                if (train.Rounds == -360)
                {
                    train.Mline = true;
                    train.Mcircle = false;
                    train.Rounds = 0;
                    rotationAngle = 0;
                }
                DrawDubb(this.CreateGraphics());
            }
        }
        void MOnLine()
        {
            if (train.Mline && pointerL < lines.Count)
            {
                if (lines[pointerL].CalcNextPoint())
                {
                    train.X = (int)lines[pointerL].cx * ts;
                    train.Y = (int)lines[pointerL].cy;
                }
                else if (pointerL < lines.Count)
                {
                    pointerL++;
                }
            }
        }

        void CatchCircle()
        {
            for (int i = 0; i < circles.Count; i++)
            {
                if (train.X == circles[i].XC && train.Rounds == 0)
                {
                    train.Mcircle = true;
                    train.Mline = false;
                    train.posC = i;
                }

            }
        }
        //Creation
        void CreateMap()
        {
            Map pnn = new Map();
            pnn.img = new Bitmap("bb2.jpeg");
            //pnn.X = 10;
            // pnn.Y = 100;
            pnn.rcSrc = new Rectangle(0, 0, pnn.img.Width, pnn.img.Height);
            pnn.rcDst = new Rectangle(0, 0, pnn.img.Width, pnn.img.Height);
            Lmap.Add(pnn);
        }
        void CreateLine()
        {
            DDA temp = new DDA();
            temp.X = pointerX;
            temp.Y =   ClientSize.Height-125;
            temp.Xe = pointerX + 100;
            temp.Ye =   ClientSize.Height-125;
            temp.calc();
            //3han akon 3arf in el line etrasm
            ptstart = new Point((int)temp.X, (int)temp.Y);
            ptend = new Point((int)temp.Xe, (int)temp.Ye);
            ////////////////////////////////////////////

            pointerX = (int)temp.Xe;
            lines.Add(temp);
        }
        void CreateCircle()
        {
            DDA temp = new DDA();
            temp.X = pointerX;
            temp.Y =   ClientSize.Height-125;
            temp.Ye =   ClientSize.Height-125;
            temp.calc();
            lines.Add(temp);
            //Creating the circle
            Circle tempc = new Circle();
            tempc.Rad = 100;
            temp.Xe = temp.X + tempc.Rad + 20;
            tempc.XC = (int)((temp.X + temp.Xe) / 2);
            tempc.YC = (   ClientSize.Height-125) - tempc.Rad;
            circles.Add(tempc);
            ////////
            pointerX = (int)temp.Xe;
            ptstart = new Point((int)temp.X, (int)temp.Y);
            ptend = new Point((int)temp.Xe, (int)temp.Ye);
        }
        void CreateCurve()
        {
            BezierCurve c = new BezierCurve();
            Point pstart = new Point(pointerX,   ClientSize.Height-125);
            c.SetControlPoint(pstart);
            Point pmid = new Point(pointerX + 100,   ClientSize.Height-125 - 300);
            c.SetControlPoint(pmid);
            Point pend = new Point(pointerX + 200,   ClientSize.Height-125);
            c.SetControlPoint(pend);
            pointerX = pend.X;
            curves.Add(c);
        }
        ///////////////////////////      
        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            CreateMap();
            img = new Bitmap("here comes the choo choo train.gif");

            //MessageBox.Show("" + Screen.PrimaryScreen.Bounds.Width);
            //MessageBox.Show("" + Screen.PrimaryScreen.Bounds.Height);
            //MessageBox.Show("" + Screen.PrimaryScreen.Bounds.Size);



        }
        void DrawScene(Graphics g2)
        {
            g2.Clear(Color.White);
            ////////////////////////////// 
            ///360 Scrolling
            for (int i = 0; i < Lmap.Count; i++)
            {
                if (Lmap[i].rcSrc.X + ClientSize.Width > Lmap[i].img.Width)
                {
                    //1
                    int cxRem = Lmap[i].img.Width - Lmap[i].rcSrc.X;
                    Rectangle Dst1 = new Rectangle(0, 0, cxRem, ClientSize.Height);
                    Rectangle Src1 = new Rectangle(Lmap[i].rcSrc.X, 0, cxRem, ClientSize.Height);
                    g2.DrawImage(Lmap[i].img, Dst1, Src1, GraphicsUnit.Pixel);
                    //2
                    int cxRem2 = ClientSize.Width - cxRem;
                    Rectangle src2 = new Rectangle(0, 0, cxRem2, ClientSize.Height);
                    Rectangle Dst2 = new Rectangle(cxRem, 0, cxRem2, ClientSize.Height);
                    g2.DrawImage(Lmap[i].img, Dst2, src2, GraphicsUnit.Pixel);
                }
                else
                {
                    g2.DrawImage(Lmap[i].img, Lmap[i].rcDst, Lmap[i].rcSrc, GraphicsUnit.Pixel);
                }
            }
            Pen p = new Pen(Brushes.White, 10);
            // Draw Map
            for (int i = 0; i < Lmap.Count; i++)
            {
                g2.DrawImage(Lmap[i].img, Lmap[i].rcDst, Lmap[i].rcSrc, GraphicsUnit.Pixel);
            }
            for (int i = 0; i < lines.Count; i++)
            {
                g2.DrawLine(p, lines[i].X, lines[i].Y, lines[i].Xe, lines[i].Ye);
            }
            if (!animation && lines.Count > 0)
            {
                g2.FillEllipse(Brushes.BlueViolet, ptstart.X - 5, ptstart.Y - 5, 10, 10);
                g2.FillEllipse(Brushes.BlueViolet, ptend.X - 5, ptend.Y - 5, 10, 10);
            }
            for (int i = 0; i < circles.Count; i++)
            {
                circles[i].Drawcircle(g2);
            }
            for (int i = 0; i < curves.Count; i++)
            {
                curves[i].DrawCurve(g2, animation);
            }
            if (animation)
            {
                //rotate the image 
                Bitmap img = new Bitmap("here comes the choo choo train.gif");
                Bitmap bmp = new Bitmap(img.Width, img.Height);
                Graphics g = Graphics.FromImage(bmp);

                // Now set the rotation point to the center of our image
                g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

                // Now rotate the image
                g.RotateTransform(train.Rounds);

                // Now return the transformation we applied
                g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

                // Now draw the new image
                g.DrawImage(img, new Point(0, 0));

                // Draw the rotated image at the train's position
                g2.DrawImage(bmp, train.X - bmp.Width/2 , train.Y - bmp.Height);
            }
            if (!animation)
            {
                string instructions = "1: Line   2: Circle    3: Curve   D: Delete last object";
                Font font = new Font("Arial", 16);
                Brush brush = Brushes.White;
                PointF point = new PointF(10, 10);
                g2.DrawString(instructions, font, brush, point);
                g2.DrawString("Last Object: " + obj, font, brush, 1000, 10);
            }
            Font fon = new Font("Arial", 16);

            g2.DrawString("SIM MODE: " + animation, fon, Brushes.White, 650, 10);
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
        //mod
        void ModifyLines(int flag)
        {
            if (lines.Count == 0)
                return;
            switch (flag)
            {
                //edit on the xe
                //بنعدل على اخر واحد في الليست عشان هو اللي لسه محطوط
                case 0:
                    lines[lines.Count - 1].Xe += 10;
                    ptend.X =(int) lines[lines.Count - 1].Xe;
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    break;
                case 1:
                    lines[lines.Count - 1].Xe -= 10;
                    ptend.X = (int)lines[lines.Count - 1].Xe;
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    break;
            }
        }
        void ModifyCircle(int flag)
        {
            if (circles.Count == 0)
                return;

            switch (flag)
            {
                //edit on the radeus of the circle 
                case 0:
                    circles[circles.Count - 1].Rad += 10;
                    circles[circles.Count - 1].XC += 10;
                    circles[circles.Count - 1].YC -= 10;
                    lines[lines.Count - 1].Xe += 20;
                    pointerX =(int) lines[lines.Count - 1].Xe;
                    ptstart = new Point((int)lines[lines.Count - 1].X, (int)lines[lines.Count - 1].Y);
                    ptend = new Point((int)lines[lines.Count - 1].Xe, (int)lines[lines.Count - 1].Ye);
                    break;
                case 1:
                    circles[circles.Count - 1].Rad -= 10;
                    circles[circles.Count - 1].XC -= 10;
                    circles[circles.Count - 1].YC += 10;
                    lines[lines.Count - 1].Xe -= 20;
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    ptstart = new Point((int)lines[lines.Count - 1].X, (int)lines[lines.Count - 1].Y);
                    ptend = new Point((int)lines[lines.Count - 1].Xe, (int)lines[lines.Count - 1].Ye);
                    break;
            }
        }
        void ModifyCurve(int flag)
        {
            if (curves.Count == 0)
                return;
            switch (flag)
            {//دي مشكله في سي شارب مينفعش نعل النقطه على طول لازم نعمل اوفر رايد
                case 0:
                    Point temp = curves[curves.Count - 1].ControlPoints[1];
                    temp.Y -= 10;
                    //curve with points start,mid ,end
                    //edit on the mid to increament or decrment the height
                    curves[curves.Count - 1].ControlPoints[1]=temp;
                    pointerX = curves[curves.Count - 1].ControlPoints[2].X;
                    break;
                case 1:
                    Point temp2= curves[curves.Count - 1].ControlPoints[1];
                    temp2.Y += 10;
                    curves[curves.Count - 1].ControlPoints[1] = temp2;
                    pointerX = curves[curves.Count - 1].ControlPoints[2].X;
                    break;
            }
        }
        void DeleteLastObject()
        {
            string temp = Objects[Objects.Count - 1];
            switch(temp)
            {
                case "line":
                    Objects.RemoveAt(Objects.Count - 1);
                    lines.RemoveAt(lines.Count - 1);
                    break;
                case "circle":
                    Objects.RemoveAt(Objects.Count - 1);
                    circles.RemoveAt(circles.Count - 1);
                    lines.RemoveAt(lines.Count - 1);
                    break;
                case "curve":
                    Objects.RemoveAt(Objects.Count - 1);
                    curves.RemoveAt(curves.Count - 1);
                    break;
            }

            //to update the end and ptstart and ptend 
            switch(Objects[Objects.Count-1])
            {
                case "line":
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    ptstart.X=(int) lines[lines.Count - 1].X;
                    ptend.X = (int)lines[lines.Count - 1].Xe;
                    break;
                case "circle":
                    pointerX = (int)lines[lines.Count - 1].Xe;
                    ptstart.X = (int)lines[lines.Count - 1].X;
                    ptend.X = (int)lines[lines.Count - 1].Xe;
                    break;
                case "curve":
                    pointerX = (int)curves[curves.Count - 1].ControlPoints[2].X;
                    ptstart.X = (int)lines[lines.Count - 1].X;
                    ptend.X = (int)lines[lines.Count - 1].Xe;
                    break;
            }
            obj = Objects[Objects.Count - 1];
            DrawDubb(this.CreateGraphics());
        }
        private void ScrollMapRight()
        {
            int scrollSpeed = 20;

            foreach (var map in Lmap)
            {
                map.rcSrc.X += scrollSpeed;
                if (map.rcSrc.X >= map.img.Width)
                {
                    map.rcSrc.X = 0;
                }
            }
            While_Scrolling(scrollSpeed, -1);
        }


    }
}
