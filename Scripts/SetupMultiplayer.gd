@tool
extends EditorScript

# This script helps set up the multiplayer scenes correctly
# Run it from the FileSystem dock: right-click > Change Script > attach this script > Run in editor

func _run():
	print("Setting up multiplayer scenes...")
	
	# Load the scenes
	var game_scene = load("res://Scenes/Game.tscn")
	var player_scene = load("res://Scenes/Player.tscn")
	
	if game_scene == null:
		print("Error: Game.tscn not found!")
		return
		
	if player_scene == null:
		print("Error: Player.tscn not found!")
		return
	
	# Modify Game scene
	var game_root = game_scene.instantiate()
	
	# Check if GameManager already exists
	var game_manager = game_root.get_node_or_null("GameManager")
	if game_manager == null:
		game_manager = Node.new()
		game_manager.name = "GameManager"
		game_manager.set_script(load("res://Scripts/GameManager.cs"))
		game_root.add_child(game_manager)
		game_manager.owner = game_root
		print("Added GameManager to Game scene")
	
	# Set PlayerScene reference
	if game_manager.has_method("set"):
		game_manager.set("PlayerScene", player_scene)
		print("Set PlayerScene reference in GameManager")
	
	# Check if MultiplayerDebugger exists
	var debugger = game_root.get_node_or_null("MultiplayerDebugger")
	if debugger == null:
		debugger = Control.new()
		debugger.name = "MultiplayerDebugger" 
		debugger.set_script(load("res://Scripts/MultiplayerDebugger.cs"))
		debugger.set_anchors_and_offsets_preset(Control.PRESET_TOP_LEFT)
		debugger.size = Vector2(200, 300)
		game_root.add_child(debugger)
		debugger.owner = game_root
		print("Added MultiplayerDebugger to Game scene")
	
	# Save modified Game scene
	var packed_game = PackedScene.new()
	packed_game.pack(game_root)
	ResourceSaver.save(packed_game, "res://Scenes/Game.tscn")
	print("Saved modified Game.tscn")
	
	# Clean up
	game_root.queue_free()
	
	print("Multiplayer setup complete!")
	print("Don't forget to:")
	print("1. Add NetworkManager as autoload in Project Settings")
	print("2. Set up input map with required actions")
	print("3. Assign PlayerScene property in GameManager export")
