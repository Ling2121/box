using Godot;
using System.Collections.Generic;

namespace Box.DataCanvas {
    public static class DataCanvasUtil {
        public static int[] ToPointArray(Vector2[] arr) {
            var to_arr = new int[arr.Length * 2];

            int n = 0;
            foreach(var p in arr) {
                to_arr[n] = (int)p.x;
                to_arr[n+1] = (int)p.y;
                n+=2;
            }

            return to_arr;
        }

        public static int[] ToPointArray(List<Vector2> arr) {
            return ToPointArray(arr.ToArray());


            
        }

        public static ImageTexture ToImageTexture<DataType>(IDataCanvas<DataType> canvas)  where DataType : struct  {
            ImageTexture texture = new ImageTexture();
            Image data = new Image();

            Dictionary<DataType,Color> colors = new Dictionary<DataType, Color>();

            data.Create(canvas.Width,canvas.Height,false,Image.Format.Rgba8);

            data.Lock();

            for(int y = 0;y < canvas.Height;y++) {
                for(int x = 0;x < canvas.Width;x++) {
                    DataType d = canvas[x,y];
                    Color color;
                    if(colors.ContainsKey(d )) {
                        color = colors[d];
                    } else {
                        color = new Color((float)GD.RandRange(0.0,1.0),(float)GD.RandRange(0.0,1.0),(float)GD.RandRange(0.0,1.0));
                        colors[d] = color;
                    }
                    data.SetPixel(x,y,color);
                }
            }

            data.Unlock();
            
            texture.CreateFromImage(data);
            return texture;
        }
    }
}