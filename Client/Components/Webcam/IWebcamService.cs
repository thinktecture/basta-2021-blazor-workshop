using System.Threading.Tasks;

namespace ConfTool.Client.Components.Webcam
{
    public interface IWebcamService
    {
        Task StartVideoAsync(WebcamOptions options);
        Task TakePictureAsync();
    }
}
