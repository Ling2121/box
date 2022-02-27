using System;
using System.Drawing;
using System.Drawing.Imaging;
using GD = Godot.GD;

namespace Box.DataCanvas.SystemDrawing {
    public class DataCanvas16Bit : IDataCanvas<ushort> {

        // B G R A
        protected ushort ColorToData(Color color) {
            ushort value =  (ushort)color.R;
            value |= (ushort)(((ushort)color.G) << 8);
            return value;
        }

        protected Color DataToColor(ushort data) {
            int r = (0x00FF & data);
            int g = ((0xFF00 & data) >> 8);
            return Color.FromArgb(255,r,g,255);
        }

        public ushort this[int x,int y]{
            get { return ColorToData(canvas.GetPixel(x,y)); }
            set { canvas.SetPixel(x,y,DataToColor(value)); }
        }

        public int Width {get {return canvas.Width;}}
        public int Height {get {return canvas.Height;}}

        Bitmap canvas;
        Graphics graphics;

        public DataCanvas16Bit(int width = 512,int height = 512) {
            canvas = new Bitmap(width,height);
            graphics = Graphics.FromImage(canvas);
            graphics.Clear(Color.White);
        }

        public Godot.Color ToColor(ushort data) {
            byte r = (byte)(0x00FF & data);
            byte g = (byte)((0xFF00 & data) >> 8);
            return Godot.Color.Color8(255,r,g,255);
        }
        public ushort ToData(Godot.Color color) {
            ushort value =  (ushort)color.r;
            value |= (ushort)(((ushort)color.g) << 8);
            return value;
        }

        public IDataCanvas<ushort> DrawBegin() {
            graphics.Clear(Color.White);
            return this;
        }
        public IDataCanvas<ushort> DrawEnd(){
            return this;
        }
        public IDataCanvas<ushort> DrawPoint(int x,int y,ushort data){
            this[x,y] = data;
            return this;
        }
        public IDataCanvas<ushort> DrawLine(int x1,int y1,int x2,int y2,float width,ushort data){
            graphics.DrawLine(new Pen(DataToColor(data),width),x1,y1,x2,y2);
            return this;
        }
        public IDataCanvas<ushort> DrawRectangle(int x,int y,int width,int height,bool is_fill,ushort data){
            graphics.DrawRectangle(new Pen(DataToColor(data),1),x,y,width,height);
            return this;
        }
        public IDataCanvas<ushort> DrawPolygon(int[] points,ushort data,bool is_fill){
            Point[] points2 = new Point[points.Length / 2];
            int n = 0;
            for(int i = 0;i<points.Length;i+=2)
            {
                points2[n] = new Point(points[i],points[i + 1]);
                n++; 
            }
            
            if(is_fill)
            {
                graphics.FillPolygon(new SolidBrush(DataToColor(data)),points2);
            }
            else
            {
                graphics.DrawPolygon(new Pen(DataToColor(data),1),points2);
            }
            return this;
        }
    }
}