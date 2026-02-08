class_name Log

static func debug(message: String):
	if OS.is_debug_build():
		print_rich("[color=cyan][DEBUG][/color] %s" % message)

static func info(message: String):
	if OS.is_debug_build():
		print_rich("[color=green][INFO][/color] %s" % message)

static func warn(message: String):
	# print_rich allows BBCode coloring in the Output panel
	print_rich("[color=yellow][WARN][/color] %s" % message)
	if OS.is_debug_build():
		push_warning(message) # Adds a yellow triangle to the debugger

static func error(message: String):
	print_rich("[color=red][ERROR][/color] %s" % message)