using System.Collections;
using Godot;
using System;


namespace Box {
    public class StorageFileTestScene : Node2D
    {

        public string ArrToStr(ArrayList arr) {
            string str = "{ ";
            foreach(object v in arr) {
                str += v.ToString() + " ";
            }
            str += "}";
            return str;
        }

        public string HashToStr(Hashtable hash) {
            string str = "{ ";
            foreach(string key in hash.Keys) {
                str += $"{key}:{hash[key]} ";
            }
            str += "}";
            return str;
        }

        public void Test(string file_name,IStorageFile file) {

            if(file.Open(file_name,StorageFileMode.Write)) {
                GD.Print("文件打开成功，开始写入...");

                GD.Print("数字写入");
                file.WriteItem(2333); GD.Print("    写入:",2333);
                file.WriteItem(-2333); GD.Print("   写入:",-2333);
                file.WriteItem(Int32.MaxValue); GD.Print("  写入:",Int32.MaxValue);
                file.WriteItem(2.333); GD.Print("    写入:",2.333);
                file.WriteItem(-2.333); GD.Print("    写入:",-2.333);
                
                GD.Print("字符串写入");
                file.WriteItem("123abcde#4%!$NSQWQ");GD.Print("    写入:","123abcde#4%!$NSQWQ");
                file.WriteItem("中文，你好，Godot，戈多");GD.Print("    写入:","中文，你好，Godot，戈多");

                GD.Print("数组写入");
                ArrayList list1 = new ArrayList();
                ArrayList list2 = new ArrayList();
                ArrayList list3 = new ArrayList();
                int len = 10;
                for(int i = 0;i < len;i++) {
                    list1.Add(233);
                    list2.Add(2.333);
                    list3.Add("字符串");
                }
                file.WriteItem(list1);GD.Print("    写入整型数字数组，长度：",len);
                file.WriteItem(list2);GD.Print("    写入浮点数字数组，长度：",len);
                file.WriteItem(list3);GD.Print("    写入浮字符串数组，长度：",len);

                GD.Print("哈希表写入");
                Hashtable table1 = new Hashtable{
                    {"Key1",2333},
                    {"Key2",2.333},
                    {"Key3","这是一个中文字符串"},
                    {"Key4","abcdefg12314q...#%#%"},
                    {"Key5",list1},
                    {"Key6",list3}
                };
                file.WriteItem(table1);GD.Print("    写入哈希表");
                file.Close();
                GD.Print("写入完成");
            }

            GD.Print("----------------------");


            if(file.Open(file_name,StorageFileMode.Read)) {
                GD.Print("文件打开成功，开始读取...");
                GD.Print("  Read:",file.TryReadIntItem());
                GD.Print("  Read:",file.TryReadIntItem());
                GD.Print("  Read:",file.TryReadIntItem());
                GD.Print("  Read:",file.TryReadFloatItem());
                GD.Print("  Read:",file.TryReadFloatItem());
                GD.Print("   ");
                GD.Print("  Read:",file.TryReadStringItem());
                GD.Print("  Read:",file.TryReadStringItem());
                GD.Print("   ");
                GD.Print("  Read:",ArrToStr(file.TryReadArrayItem()));
                GD.Print("  Read:",ArrToStr(file.TryReadArrayItem()));
                GD.Print("  Read:",ArrToStr(file.TryReadArrayItem()));
                GD.Print("   ");
                GD.Print("  Read:",HashToStr(file.TryReadHashtableItem()));
                file.Close();
                GD.Print("读取完成");
            }
        }

        public override void _Ready()
        {
            GDIOStorageFile file = new GDIOStorageFile();
            GD.Print(nameof(GDIOStorageFile));
            Test("test/storage_file_test/gdio.dat",new GDIOStorageFile());
            GD.Print(nameof(SYSIOStorageFile));
            Test("test/storage_file_test/sysio.dat",new SYSIOStorageFile());

            GD.Print("GDIOStorageFile read SYSIOStorageFile");
            if(file.Open("test/storage_file_test/sysio.dat",StorageFileMode.Read)) {
                GD.Print("文件打开成功，开始读取...");
                GD.Print("  Read:",file.TryReadIntItem());
                GD.Print("  Read:",file.TryReadIntItem());
                GD.Print("  Read:",file.TryReadIntItem());
                GD.Print("  Read:",file.TryReadFloatItem());
                GD.Print("  Read:",file.TryReadFloatItem());
                GD.Print("   ");
                GD.Print("  Read:",file.TryReadStringItem());
                GD.Print("  Read:",file.TryReadStringItem());
                GD.Print("   ");
                GD.Print("  Read:",ArrToStr(file.TryReadArrayItem()));
                GD.Print("  Read:",ArrToStr(file.TryReadArrayItem()));
                GD.Print("  Read:",ArrToStr(file.TryReadArrayItem()));
                GD.Print("   ");
                GD.Print("  Read:",HashToStr(file.TryReadHashtableItem()));
                file.Close();
                GD.Print("读取完成");
            }
        }
    }
}
