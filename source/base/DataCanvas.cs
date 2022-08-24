using SkiaSharp;
using Vector2 = Godot.Vector2;
using Rect2 = Godot.Rect2;
using ImageTexture = Godot.ImageTexture;
using Image = Godot.Image;

namespace Box {
    public class DataCanvas {
        public static SKPoint ToSKPoint(Vector2 p) {
            return new SKPoint(p.x,p.y);
        }

        public static SKPoint[] ToSKPointArray(Vector2[] points) {
            SKPoint[] arr = new SKPoint[points.Length];
            for(int i = 0;i<points.Length;i++) {
                arr[i] = new SKPoint(points[i].x,points[i].y);
            }
            return arr;
        }


        public readonly int Width;
        public readonly int Height;

        protected SKBitmap Bitmap;
        protected SKCanvas Canvas;

        SKPaint LinePaint = new SKPaint {
            Style = SKPaintStyle.Stroke,
        };
        SKPaint FillPaint = new SKPaint {
            Style = SKPaintStyle.Fill
        };

        public uint this[int x,int y] {
            get {
                return GetPixel(x,y);
            }
        }

        public DataCanvas(){}

        public DataCanvas(int w,int h) {
            Width = w;
            Height = h;
            Bitmap = new SKBitmap(w,h,SKColorType.Rgba8888,SKAlphaType.Premul);
            Canvas = new SKCanvas(Bitmap);            

        }

        public void DrawLine(uint data,Vector2 p0,Vector2 p1) {
            LinePaint.Color = data;
            Canvas.DrawLine(ToSKPoint(p0),ToSKPoint(p1),LinePaint);
        }

        public void DrawRectangle(uint data,Rect2 rect,bool is_fill) {
            float x = rect.Position.x;
            float y = rect.Position.y;
            float w = rect.Size.x;
            float h = rect.Size.y;
            SKPaint paint = is_fill ? FillPaint : LinePaint;
            paint.Color = data;
            Canvas.DrawRect(x,y,w,h,paint);
        }
        
        public void DrawCircle(uint data,Vector2 p,float r,bool is_fill) {
            SKPaint paint = is_fill ? FillPaint : LinePaint;
            paint.Color = data;
            Canvas.DrawCircle(p.x,p.y,r,paint);
        }

        public  void DrawPolygon(uint data,Vector2[] points,bool is_fill) {
            SKPath path = new SKPath();
            path.AddPoly(ToSKPointArray(points));
            SKPaint paint = is_fill ? FillPaint : LinePaint;
            paint.Color = data;
            Canvas.DrawPath(path,paint);
        }

        public uint GetPixel(int x,int y) {
            return (uint)Bitmap.GetPixel(x,y);
        }

        public ImageTexture ToImage() {
            ImageTexture imageTexture = new ImageTexture();
            Image image = new Image();

            image.Create(Width,Height,true,Godot.Image.Format.Rgba8);
            image.Lock();

            for(int y = 0;y < Width;y++) {
                for(int x = 0;x < Height;x++) {
                    var color = Bitmap.GetPixel(x,y);
                    image.SetPixel(x,y,new Godot.Color(color.Red,color.Green,color.Blue,color.Alpha));
                }
            }

            image.Unlock();
            imageTexture.CreateFromImage(image);
            return imageTexture;
        }
    }
}