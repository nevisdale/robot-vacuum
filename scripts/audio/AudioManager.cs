using Godot;
using Godot.Collections;
using RobotVacuum.Scripts.Globals;

namespace RobotVacuum.Scripts.Audio;

public partial class AudioManager : AudioStreamPlayer
{
    public enum BackgroundSound
    {
        Unknown = -1,
        Alone = 0,
        Couple = 1,
        Family = 2,
        Divorce = 3,
    }

    public static AudioManager Instance { get; private set; }

    public int MusicVolume
    {
        get
        {
            SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
            return gameState.MusicVolume;
        }
    }

    public int SoundVolume
    {
        get
        {
            SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
            return gameState.SoundVolume;
        }
    }

    // main intro outro theme
    [Export]
    private AudioStreamPlayer _introTheme = null;

    // background sounds
    [Export]
    private AudioStream _aloneBackgroundSound = null;
    [Export]
    private AudioStream _coupleBackgroundSound = null;
    [Export]
    private AudioStream _familyBackgroundSound = null;
    [Export]
    private AudioStream _divorceBackgroundSound = null;

    // sfxs
    [Export]
    private AudioStreamPlayer _pushGarbageSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _captureGarbageSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _catCaptureSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _carCaptureSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _activateButtonSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _getElectrricitySoundPlayer = null;
    [Export]
    private AudioStreamPlayer _wetSpotSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _binCaptureGarbageSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _nextLevelOkSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _nextLevelErrorSoundPlayer = null;


    private Dictionary<BackgroundSound, AudioStream> _backgroundSounds = null;
    private BackgroundSound _currentBackgroundSound = BackgroundSound.Unknown;

    public override void _Ready()
    {
        Stop();

        ApplyVolume();

        _backgroundSounds = new Dictionary<BackgroundSound, AudioStream>
        {
            { BackgroundSound.Alone, _aloneBackgroundSound },
            { BackgroundSound.Couple, _coupleBackgroundSound },
            { BackgroundSound.Family, _familyBackgroundSound },
            { BackgroundSound.Divorce, _divorceBackgroundSound }
        };
        Stream = _backgroundSounds[BackgroundSound.Alone];

        Instance = this;
    }

    public void ForceStop()
    {
        // background sound
        Stop();

        // sfx
        _pushGarbageSoundPlayer?.Stop();
        _captureGarbageSoundPlayer?.Stop();
        _catCaptureSoundPlayer?.Stop();
        _carCaptureSoundPlayer?.Stop();
        _currentBackgroundSound = BackgroundSound.Unknown;

        // intro theme
        _introTheme?.Stop();
    }

    public void PlaySoundBackgroundType(BackgroundSound backgroundSoundType)
    {
        if (_currentBackgroundSound == backgroundSoundType)
        {
            return;
        }

        StopSoundBackground();
        _currentBackgroundSound = backgroundSoundType;
        Stream = _backgroundSounds[_currentBackgroundSound];
        PlaySoundBackground();
    }

    public void PlaySoundBackground()
    {
        if (!Playing)
        {
            Play();
        }
    }

    public void StopSoundBackground()
    {
        Stop();
        _currentBackgroundSound = BackgroundSound.Unknown;
    }

    public async void StopSoundBackgroundSmooth()
    {
        if (!Playing)
        {
            return;
        }

        Tween tween = CreateTween();
        tween.TweenProperty(this, "volume_db", -80, 5);
        tween.Finished += () =>
        {
            Stop();
        };
        await ToSignal(tween, "finished");

        // restore volume
        ApplyVolume();
        _currentBackgroundSound = BackgroundSound.Unknown;
    }

    // Robot captures garbage
    public void PlaySound_RobotCaptureGarbage() => PlaySound(_captureGarbageSoundPlayer, startFrom: 0.2f);

    // Robot pushes garbage
    public void PlaySound_RobotPushGarbage() => PlaySound(_pushGarbageSoundPlayer);

    // Cat captures robot
    public void PlaySound_CatCaptureRobot() => PlaySound(_catCaptureSoundPlayer);

    // Car captures robot
    public void PlaySound_CarCaptureRobot() => PlaySound(_carCaptureSoundPlayer, 0.1f);

    // Activate button
    public void PlaySound_ActivateButton() => PlaySound(_activateButtonSoundPlayer);

    // Receive electricity
    public void PlaySound_GetElectricity() => PlaySound(_getElectrricitySoundPlayer);

    // On wet spot
    public void PlaySound_WetSpot() => PlaySound(_wetSpotSoundPlayer);

    // Bin captures garbage
    public void PlaySound_BinCaptureGarbage() => PlaySound(_binCaptureGarbageSoundPlayer);

    // Next level ok
    public void PlaySound_NextLevelOk() => PlaySound(_nextLevelOkSoundPlayer);

    // Next level error
    public void PlaySound_NextLevelError() => PlaySound(_nextLevelErrorSoundPlayer);

    private async void PlaySound(AudioStreamPlayer audioStreamPlayer, float startFrom = 0f)
    {
        if (!audioStreamPlayer.Playing)
        {
            audioStreamPlayer.Stop();
            audioStreamPlayer.Play(startFrom);
            await ToSignal(audioStreamPlayer, "finished");
        }
    }

    private void ApplyVolume()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();

        float musicVolumeDb = gameState.MusicVolumeDb();
        VolumeDb = musicVolumeDb;
        _introTheme.VolumeDb = musicVolumeDb;

        float soundVolumeDb = gameState.SoundVolumeDb();
        _pushGarbageSoundPlayer.VolumeDb = soundVolumeDb;
        _captureGarbageSoundPlayer.VolumeDb = soundVolumeDb;
        _catCaptureSoundPlayer.VolumeDb = soundVolumeDb;
        _carCaptureSoundPlayer.VolumeDb = soundVolumeDb;
        _activateButtonSoundPlayer.VolumeDb = soundVolumeDb;
        _getElectrricitySoundPlayer.VolumeDb = soundVolumeDb;
        _wetSpotSoundPlayer.VolumeDb = soundVolumeDb;
        _binCaptureGarbageSoundPlayer.VolumeDb = soundVolumeDb;
        _nextLevelOkSoundPlayer.VolumeDb = soundVolumeDb;
        _nextLevelErrorSoundPlayer.VolumeDb = soundVolumeDb;
    }

    public AudioStreamPlayer GetIntroTheme()
    {
        return _introTheme;
    }

    public void MusicVolumeUp()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.MusicVolumeUp();
        SaveManager.Instance.SaveGameState();
        ApplyVolume();
    }

    public void MusicVolumeDown()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.MusicVolumeDown();
        SaveManager.Instance.SaveGameState();
        ApplyVolume();
    }

    public void SoundVolumeUp()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.SoundVolumeUp();
        SaveManager.Instance.SaveGameState();
        ApplyVolume();
    }

    public void SoundVolumeDown()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.SoundVolumeDown();
        SaveManager.Instance.SaveGameState();
        ApplyVolume();
    }
}
