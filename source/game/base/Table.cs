using System;
using System.Collections;

namespace Box {
    public class Table : Hashtable {
        public T GetValue<T>(string key) {
            if(!ContainsKey(key)) return default(T);
            return (T)this[key];
        }

        public T GetValue<T>(string key,T else_value) {
            if(!ContainsKey(key)) return else_value;
            return (T)this[key];
        }

        public T GetValue<T>(string key,Func<T> else_value_f) {
            if(!ContainsKey(key)) return else_value_f();
            return (T)this[key];
        }

        public Table SetValue(string key,object value) {
            if(value == null)
            {
                Remove(key);
                return this;
            }
            this[key] = value;

            return this;
        }

        public Table SetValue(string key,Func<object> value_f) {
            this[key] = value_f();

            return this;
        }

        public Table SetValueFrom(Table t,string key,object else_value) {
            return SetValue(key,t.ContainsKey(key) ? t[key] : else_value);
        }

        public Table SetValueFrom(Table t,string key,Func<object> else_value_f) {
            return SetValue(key,t.ContainsKey(key) ? t[key] : else_value_f());
        }

        public Table SetValueFromSelf(string key,object else_value) {
            if(!ContainsKey(key)) {
                SetValue(key,else_value);
            }
            return this;
        }

        public Table SetValueFromSelf(string key,Func<object> else_value_f) {
            if(!ContainsKey(key)) {
                SetValue(key,else_value_f());
            }
            return this;
        }

        public Table SetValue<T>(string key,object value) {
            if(value == null)
            {
                Remove(key);
                return this;
            }
            this[key] = value;

            return this;
        }

        public Table SetValue<T>(string key,Func<object> value_f) {
            this[key] = value_f();

            return this;
        }

        public Table SetValueFrom<T>(Table t,string key,object else_value) {
            return SetValue<T>(key,t.ContainsKey(key) ? t[key] : else_value);
        }

        public Table SetValueFrom<T>(Table t,string key,Func<object> else_value_f) {
            return SetValue<T>(key,t.ContainsKey(key) ? t[key] : else_value_f());
        }

        public Table SetValueFromSelf<T>(string key,object else_value) {
            if(!ContainsKey(key)) {
                SetValue<T>(key,else_value);
            }
            return this;
        }

        public Table SetValueFromSelf<T>(string key,Func<object> else_value_f) {
            if(!ContainsKey(key)) {
                SetValue<T>(key,else_value_f());
            }
            return this;
        }


    }
}