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
        Stop();
        _pushGarbageSoundPlayer.Stop();
        _captureGarbageSoundPlayer.Stop();
        _catCaptureSoundPlayer.Stop();
        _carCaptureSoundPlayer.Stop();
        _currentBackgroundSound = BackgroundSound.Unknown;
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

    public void PlaySoundPushGarbage()
    {
        PlaySound(_pushGarbageSoundPlayer);
    }

    public void PlaySoundCaptureGarbage()
    {
        PlaySound(_captureGarbageSoundPlayer, startFrom: 0.2f);
    }

    public void PlaySoundCatCapture()
    {
        PlaySound(_catCaptureSoundPlayer, 0.2f);
    }

    public void PlaySoundCarCapture()
    {
        PlaySound(_carCaptureSoundPlayer, 0.1f);
    }

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
        VolumeDb = gameState.MusicVolumeDb();

        float soundVolumeDb = gameState.SoundVolumeDb();
        _pushGarbageSoundPlayer.VolumeDb = soundVolumeDb;
        _captureGarbageSoundPlayer.VolumeDb = soundVolumeDb;
        _catCaptureSoundPlayer.VolumeDb = soundVolumeDb;
        _carCaptureSoundPlayer.VolumeDb = soundVolumeDb;
    }

    public void MusicVolumeUp()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.MusicVolumeUp();
        ApplyVolume();
    }

    public void MusicVolumeDown()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.MusicVolumeDown();
        ApplyVolume();
    }

    public void SoundVolumeUp()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.SoundVolumeUp();
        ApplyVolume();
    }

    public void SoundVolumeDown()
    {
        SaveManager.GameState gameState = SaveManager.Instance.GetGameState();
        gameState.SoundVolumeDown();
        ApplyVolume();
    }
}
