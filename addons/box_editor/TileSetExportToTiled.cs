using Godot;

namespace Box {
    public class TileSetExportToTiled : Godot.Object {
        
        public static void Export(TileSet tileset,string export_path,string export_name) {
            File file = new File();

            string root_dir = System.IO.Directory.GetCurrentDirectory();
            root_dir = root_dir.Replace('\\','/');
            string export_path_sys = $"{export_path.Substring(6)}.png";
            string png_dir = $"{root_dir}/{export_path_sys}";
            Texture texture = GD.Load<Texture>($"res://{export_path_sys}");

            if(texture == null) {
                GD.PushWarning("需要在同一目录下放置PNG类型的图片");
                return;
            }
            if(file.Open(export_path + ".tsx",File.ModeFlags.WriteRead) == Error.Ok) {
                Rect2 region = tileset.TileGetRegion(0);
                int width = (int)region.Size.x;
                int height = (int)region.Size.y;
                int tw = texture.GetWidth();
                int th = texture.GetHeight();
                int ws = tw / width;
                var ids = tileset.GetTilesIds();
                file.StoreLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                file.StoreLine($"<tileset version=\"1.8\" tiledversion=\"1.8.2\" name=\"{export_name}\" tilewidth=\"{width}\" tileheight=\"{height}\" tilecount=\"{ids.Count}\" columns=\"8\">");
                file.StoreLine($"<image source=\"{png_dir}\" width=\"{texture.GetWidth()}\" height=\"{texture.GetHeight()}\"/>");
                foreach(int id in ids) {
                    Rect2 reg = tileset.TileGetRegion(id);
                    int x = Mathf.FloorToInt(reg.Position.x / width);
                    int y = Mathf.FloorToInt(reg.Position.y / height);
                    int id2 = y * width + x;
                    file.StoreLine($"<tile id=\"{id2}\">");
                        file.StoreLine("<properties>");
                            file.StoreLine($"<property name=\"name\" value=\"{tileset.TileGetName(id)}\"/>");
                        file.StoreLine("</properties>");
                    file.StoreLine("</tile>");
                }
                file.StoreLine("</tileset>");

                file.Close();
            }
        }


        public static void ExportTiled(string path) {
            Directory dir = new Directory();
            if(dir.Open(path) == Error.Ok) {
                dir.ListDirBegin();
                string file_name = dir.GetNext();
                while(file_name != ""){
                    if(!dir.CurrentIsDir()) {
                        if(System.IO.Path.GetExtension(file_name) == ".tres") {
                            string old_path = $"{path}/{file_name}";
                            string export_name = System.IO.Path.GetFileNameWithoutExtension(file_name);
                            string export_path = $"{path}/{export_name}";
                            Resource resource = GD.Load(old_path);
                            if(resource is TileSet) {
                                GD.Print($"export : {old_path} -> {export_path}");
                                Export((TileSet)resource,export_path,export_name);
                            }
                        }
                    } else {
                        if(file_name != "." && file_name != "..") {
                            ExportTiled($"{path}/{file_name}");
                        }
                    }
                    file_name = dir.GetNext();
                }
            }
        }
    }
}