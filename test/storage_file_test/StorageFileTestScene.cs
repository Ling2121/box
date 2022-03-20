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

        public override void _Ready()
        {
            string file_name = "user://test_file.dat";
            StorageFile file = new StorageFile();
            file.Open(file_name,File.ModeFlags.WriteRead);

            if(file.IsOpen()) {
                GD.Print("文件打开成功，开始写入...");

                GD.Print("数字写入");
                file.Write(2333); GD.Print("    写入:",2333);
                file.Write(-2333); GD.Print("   写入:",-2333);
                file.Write(Int32.MaxValue); GD.Print("  写入:",Int32.MaxValue);
                file.Write(2.333); GD.Print("    写入:",2.333);
                file.Write(-2.333); GD.Print("    写入:",-2.333);
                
                GD.Print("字符串写入");
                file.Write("123abcde#4%!$NSQWQ");GD.Print("    写入:","123abcde#4%!$NSQWQ");
                file.Write("中文，你好，Godot，戈多");GD.Print("    写入:","中文，你好，Godot，戈多");

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
                file.Write(list1);GD.Print("    写入整型数字数组，长度：",len);
                file.Write(list2);GD.Print("    写入浮点数字数组，长度：",len);
                file.Write(list3);GD.Print("    写入浮字符串数组，长度：",len);

                GD.Print("哈希表写入");
                Hashtable table1 = new Hashtable{
                    {"Key1",2333},
                    {"Key2",2.333},
                    {"Key3","这是一个中文字符串"},
                    {"Key4","abcdefg12314q...#%#%"},
                    {"Key5",list1},
                    {"Key6",list3}
                };
                file.Write(table1);GD.Print("    写入哈希表");
                file.Close();
                GD.Print("写入完成");
            }

            GD.Print("----------------------");

            file.Open(file_name,File.ModeFlags.Read);

            if(file.IsOpen()) {
                GD.Print("文件打开成功，开始读取...");
                GD.Print("  Read:",file.ReadInt());
                GD.Print("  Read:",file.ReadInt());
                GD.Print("  Read:",file.ReadInt());
                GD.Print("  Read:",file.ReadFloat());
                GD.Print("  Read:",file.ReadFloat());
                GD.Print("   ");
                GD.Print("  Read:",file.ReadString());
                GD.Print("  Read:",file.ReadString());
                GD.Print("   ");
                GD.Print("  Read:",ArrToStr(file.ReadArray()));
                GD.Print("  Read:",ArrToStr(file.ReadArray()));
                GD.Print("  Read:",ArrToStr(file.ReadArray()));
                GD.Print("   ");
                GD.Print("  Read:",HashToStr(file.ReadHashtable()));
                file.Close();
                GD.Print("读取完成");
            }
        }
    }
}
