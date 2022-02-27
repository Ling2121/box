namespace Box {
    //线程安全的单例
    public class Singleton<T> where T : class,new() {
        protected static T instance = null;
        protected static readonly object locker = new object();


        public static T Instance {
            get {
                if(instance == null)
                {
                    lock(locker)
                    {
                        if(instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }

        protected Singleton(){}

    }
}