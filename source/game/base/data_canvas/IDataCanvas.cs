namespace Box {
    public interface IDataCanvas<T> where T : struct {
        T this[int x,int y]{get;set;}
        int Width {get;}
        int Height {get;}
        Godot.Color ToColor(T data);
        T ToData(Godot.Color c);
        IDataCanvas<T> DrawBegin();
        IDataCanvas<T> DrawEnd();
        IDataCanvas<T> DrawPoint(int x,int y,T data);
        IDataCanvas<T> DrawLine(int x1,int y1,int x2,int y2,float width,T data);
        IDataCanvas<T> DrawRectangle(int x,int y,int width,int height,bool is_fill,T data);
        IDataCanvas<T> DrawPolygon(int[] points,T data,bool is_fill);
    }
}