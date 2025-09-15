using System.Management;

namespace AutodartsTrainer.Services;

public class CameraService
{
    public IReadOnlyList<string> DetectConnectedCameras()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Camera' OR Service = 'usbvideo'");
            var result = new List<string>();
            foreach (var device in searcher.Get())
            {
                var name = device["Name"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    result.Add(name);
                }
            }

            return result;
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}
