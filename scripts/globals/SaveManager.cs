using Godot;

namespace RobotVacuum.Scripts.Globals;

public partial class SaveManager
{
    private const string SAVE_FILE = "user://savegame.save";

    public static void SaveGame(SceneTree sceneTree)
    {
        using var saveFile = FileAccess.Open(SAVE_FILE, FileAccess.ModeFlags.Write);
        saveFile.StoreLine(sceneTree.CurrentScene.SceneFilePath);
        GD.Print("Game saved");
    }

    public static bool CanLoadGame()
    {
        return FileAccess.FileExists(SAVE_FILE);
    }

    public static string GetScenePathToLoad()
    {
        using var saveFile = FileAccess.Open(SAVE_FILE, FileAccess.ModeFlags.Read);
        return saveFile.GetLine();
    }
}
