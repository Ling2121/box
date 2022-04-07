using System;

namespace Box.Scene.ProcessTreeEditor {
    public class StringLinker {
        public string String;

        public char this[int i] {
            get {
                return String[i];
            }
        }

        public int Length {
            get {
                return String.Length;
            }
        }

        public override string ToString()
        {
            return String;
        }

        public override int GetHashCode()
        {
            return String.GetHashCode();
        }
    }
}