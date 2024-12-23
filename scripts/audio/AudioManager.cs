using Godot;

namespace RobotVacuum.Scripts.Audio;

public partial class AudioManager : AudioStreamPlayer
{
    public static AudioManager Instance { get; private set; }

    [Export]
    private AudioStreamPlayer _pushGarbageSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _captureGarbageSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _catCaptureSoundPlayer = null;
    [Export]
    private AudioStreamPlayer _carCaptureSoundPlayer = null;

    private bool _muted = true;
    private float _initVolumeDb = 0;

    public override void _Ready()
    {
        Stop();
        _initVolumeDb = VolumeDb;
        Instance = this;
    }

    public void ForceStop()
    {
        Stop();
        _pushGarbageSoundPlayer.Stop();
        _captureGarbageSoundPlayer.Stop();
        _catCaptureSoundPlayer.Stop();
        _carCaptureSoundPlayer.Stop();
    }

    public void ToggleMute()
    {
        _muted = !_muted;
        if (_muted)
        {
            Stop();
        }
        else
        {
            Play();
        }
    }

    public void PlaySoundBackground()
    {
        if (_muted)
        {
            return;
        }

        if (!Playing)
        {
            Play();
        }
    }

    public void StopSoundBackground()
    {
        Stop();
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
        VolumeDb = _initVolumeDb;
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
        if (_muted)
        {
            return;
        }

        if (!audioStreamPlayer.Playing)
        {
            audioStreamPlayer.Stop();
            audioStreamPlayer.Play(startFrom);
            await ToSignal(audioStreamPlayer, "finished");
        }
    }
}
