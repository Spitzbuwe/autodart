using System.Media;

namespace AutodartsTrainer.Services;

public class AudioService
{
    public bool IsEnabled { get; private set; } = true;

    public void Enable(bool enabled) => IsEnabled = enabled;

    public void PlayCheckout()
    {
        if (IsEnabled)
        {
            SystemSounds.Asterisk.Play();
        }
    }

    public void PlayBust()
    {
        if (IsEnabled)
        {
            SystemSounds.Hand.Play();
        }
    }
}
