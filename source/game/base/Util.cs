namespace Box {
    public static class Util {
        public static bool PointIsNotIntersectBox(int min_x,int min_y,int max_x,int max_y,int px,int py) {
            return px < min_x || px > max_x || py < min_y || py > max_y;
        }
    }
}