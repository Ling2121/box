namespace Box {
    public class Interval<T> {
        public T Min;
        public T Max;

        public Interval(){}
        public Interval(T min,T max) {
            Min = min;
            Max = max;
        } 
    }
}