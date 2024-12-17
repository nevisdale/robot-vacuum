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

    public override void _Ready()
    {
        Instance = this;
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
}
