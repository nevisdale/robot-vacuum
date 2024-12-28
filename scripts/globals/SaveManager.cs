using System.Collections.Generic;
using Godot;

namespace RobotVacuum.Scripts.Globals;

public partial class SaveManager : Node2D
{
    private const string GAME_STATE_FILE = "user://savegame_v2.save";

    // keys for saving/loading game state
    private const string KEY_CURRENT_LEVEL = "current_level";
    private const string KEY_AVAILABLE_LEVEL = "available_level";

    public class GameState
    {
        // from which level to continue the game
        public string CurrentLevelScene;

        // all available levels in 'select' menu
        public HashSet<string> AvailableLevelScenes;

        public void UpdateCurrentSceneAndAddToAvailable(SceneTree sceneTree)
        {
            string currentScenePath = sceneTree.CurrentScene.SceneFilePath;
            CurrentLevelScene = currentScenePath;
            AvailableLevelScenes.Add(currentScenePath);
        }
    }

    private GameState _gameState;

    public static SaveManager Instance { get; private set; }

    public override void _Ready()
    {
        _gameState = new()
        {
            CurrentLevelScene = "",
            AvailableLevelScenes = new HashSet<string>()
        };
        LoadGameState();
        Instance = this;
    }

    private void LoadGameState()
    {
        if (!FileAccess.FileExists(GAME_STATE_FILE))
        {
            return;
        }

        using var saveFile = FileAccess.Open(GAME_STATE_FILE, FileAccess.ModeFlags.Read);

        while (!saveFile.EofReached())
        {
            string line = saveFile.GetLine();
            string[] parts = line.Split('=');
            if (parts.Length != 2)
            {
                continue;
            }

            string key = parts[0];
            string value = parts[1];

            switch (key)
            {
                case KEY_CURRENT_LEVEL:
                    _gameState.CurrentLevelScene = value;
                    break;
                case KEY_AVAILABLE_LEVEL:
                    _gameState.AvailableLevelScenes.Add(value);
                    break;
            }
        }
        saveFile.GetLine();
    }

    public void SaveGameState()
    {
        using var saveFile = FileAccess.Open(GAME_STATE_FILE, FileAccess.ModeFlags.Write);

        saveFile.StoreLine($"{KEY_CURRENT_LEVEL}={_gameState.CurrentLevelScene}");
        foreach (string levelScene in _gameState.AvailableLevelScenes)
        {
            saveFile.StoreLine($"{KEY_AVAILABLE_LEVEL}={levelScene}");
        }
    }

    public GameState GetGameState()
    {
        return _gameState;
    }
}
