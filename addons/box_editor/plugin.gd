tool
extends EditorPlugin

var BoxMeun = preload("res://addons/box_editor/BoxMeun.tscn").instance()
var ClassNameLoading = preload("res://addons/box_editor/ClassNameLoading.gd")
var TileSetExportToTiled : CSharpScript = load("res://addons/box_editor/TileSetExportToTiled.cs")

func _enter_tree():
	pass

func draw_tree(root : Node,layer:int,s):
	if layer == 0:
		return;
	s += "    "
		
	for n in root.get_children():
		print(s,"L ",n,"  ",n.name)
		if n.get_child_count() > 0:
			draw_tree(n,layer - 1,s)

func find_meun_h_box_container() -> HBoxContainer :
	var editor = get_editor_interface()
	var editor_scene = editor.get_viewport()
	var m: HBoxContainer
	for layer1 in editor_scene.get_children():
		for layer2 in layer1.get_children():
			for layer3 in layer2.get_children():
				for layer4 in layer3.get_children():
					if layer4 is VBoxContainer:
						for layer5 in layer4.get_children():
								if layer5 is HBoxContainer:
									var c = 0;
									for layer6 in layer5.get_children():
										if layer6 is HBoxContainer:
											for layer7 in layer6.get_children():
												c+=1
										if c > 4:
											m = layer6
											break;
	return m

func _ready():
	var editor = get_editor_interface()
	var editor_scene = editor.get_viewport()
	
	var m: HBoxContainer = find_meun_h_box_container();
	if m != null:
		if !BoxMeun.is_inside_tree():
			m.add_child(BoxMeun)
			BoxMeun.get_popup().connect("id_pressed",self,"_meun")
			
func _meun(id):
	if id == 0:
		print("导入C#节点")
		ClassNameLoading.loadcs(self)
		pass
	if id == 1:
		print("导出Tileset到Tiled")
		TileSetExportToTiled.ExportTiled("res://resource")
		pass

func _exit_tree():
	pass

func has_main_screen():
	return true


func make_visible(visible):
	pass

