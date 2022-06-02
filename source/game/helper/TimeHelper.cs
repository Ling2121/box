namespace Box {
    public static class TimeHelper {
        public static long MinuteHour(int hour) {
            return hour * 60;
        }

        public static long MinuteDay(int day) {
            return day * 24 * 60;
        }

        public static long MinuteMonth(int month) {
            return month * 30 * 24 * 60;
        }
    }
}