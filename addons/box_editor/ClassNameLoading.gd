class CSScriptFile:	
	var name
	var path
	
	func _init(name,path):
		self.name = name
		self.path = path
	

static func is_csscript(name):
	var regex = RegEx.new() 
	regex.compile(".\\.cs")
	return regex.search(name)
	
static func get_csscript_name(name:String):
	return name.substr(0,name.length()-3);
	
static func is_dir(name):
	return name != "." && name != ".."

static func get_csscripts(arr = [],path = "res://source"):
	var dir = Directory.new()
	if dir.open(path) == OK:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while (file_name != ""):
			if !dir.current_is_dir():
				var cssname = get_csscript_name(file_name)
				if is_csscript(file_name):
					var script = CSScriptFile.new(cssname,path + "/" + file_name)
					arr.append(script)
			else:
				if is_dir(file_name):
					get_csscripts(arr,path +"/"+file_name)
				pass
			file_name = dir.get_next()
	else:
		return arr
		
	return arr
	
static func is_load(code:String):
	return code.find("ClassName") != -1

static func loadcs(plugin):
	print("C#节点对象载入编辑器...")
	var editor_cs:CSharpScript = load("res://addons/box_editor/ClassNameLoading.cs")
	var editor = editor_cs.new()
	var scripts = get_csscripts()
	for script in scripts:
		var cs:CSharpScript = load(script.path)
		if is_load(cs.source_code):
			var c = cs.new();
			print("load  >",script.path);
			var info = editor.GetObjectRegistered(c)
			if info != null:
				plugin.remove_custom_type(info[0])
				var icon_path = "res://addons/box_editor/Icon/"+info[2]+".png";
				plugin.add_custom_type(info[0],info[1],cs,load(icon_path))
				
				
				
				
