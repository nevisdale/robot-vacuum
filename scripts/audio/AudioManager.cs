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

    public override void _Ready()
    {
        Stop();
        Instance = this;
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
        if (!Playing)
        {
            return;
        }

        Tween tween = CreateTween();
        tween.TweenProperty(this, "volume_db", -80, 5);
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
