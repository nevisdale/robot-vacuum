using System.Collections.Generic;
using Godot;

namespace RobotVacuum.Scripts.Globals;

public partial class SaveManager : Node2D
{
    private const string GAME_STATE_FILE = "user://savegame_v2.save";

    // keys for saving/loading game state
    private const string KEY_CURRENT_LEVEL = "current_level";
    private const string KEY_AVAILABLE_LEVEL = "available_level";
    private const string KEY_MUSIC_VOLUME = "music_volume";
    private const string KEY_SOUND_VOLUME = "sound_volume";

    public class GameState
    {
        public int MusicVolume;
        public int SoundVolume;

        private static float IntToDb(int value)
        {
            return value switch
            {
                0 => -80,
                1 => -60,
                2 => -40,
                3 => -30,
                4 => -20,
                5 => -15,
                6 => -10,
                7 => -5,
                8 => -3,
                9 => -1,
                10 => 0,
                _ => (float)0,
            };
        }

        private void AddMusicVolume(int value)
        {
            MusicVolume += value;
            Normalize();
        }

        private void AddSoundVolume(int value)
        {
            SoundVolume += value;
            Normalize();
        }

        public float MusicVolumeDb() => IntToDb(MusicVolume);
        public void MusicVolumeUp() => AddMusicVolume(1);
        public void MusicVolumeDown() => AddMusicVolume(-1);

        public float SoundVolumeDb() => IntToDb(SoundVolume);
        public void SoundVolumeUp() => AddSoundVolume(1);
        public void SoundVolumeDown() => AddSoundVolume(-1);

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

        public void Normalize()
        {
            if (MusicVolume < 0)
            {
                MusicVolume = 0;
            }
            else if (MusicVolume > 10)
            {
                MusicVolume = 10;
            }

            if (SoundVolume < 0)
            {
                SoundVolume = 0;
            }
            else if (SoundVolume > 10)
            {
                SoundVolume = 10;
            }
        }
    }

    private GameState _gameState;

    public static SaveManager Instance { get; private set; }

    public override void _Ready()
    {

        // default game state
        _gameState = new()
        {
            MusicVolume = 8,
            SoundVolume = 8,
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
                case KEY_MUSIC_VOLUME:
                    _ = int.TryParse(value, out _gameState.MusicVolume);
                    break;
                case KEY_SOUND_VOLUME:
                    _ = int.TryParse(value, out _gameState.SoundVolume);
                    break;
            }
        }
        saveFile.GetLine();
        _gameState.Normalize();
    }

    public void SaveGameState()
    {
        using var saveFile = FileAccess.Open(GAME_STATE_FILE, FileAccess.ModeFlags.Write);

        saveFile.StoreLine($"{KEY_CURRENT_LEVEL}={_gameState.CurrentLevelScene}");
        foreach (string levelScene in _gameState.AvailableLevelScenes)
        {
            saveFile.StoreLine($"{KEY_AVAILABLE_LEVEL}={levelScene}");
        }
        saveFile.StoreLine($"{KEY_MUSIC_VOLUME}={_gameState.MusicVolume}");
        saveFile.StoreLine($"{KEY_SOUND_VOLUME}={_gameState.SoundVolume}");
    }

    public GameState GetGameState()
    {
        return _gameState;
    }
}
