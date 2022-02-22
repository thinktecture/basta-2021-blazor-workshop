namespace ConfTool.Client.Components.Webcam
{
    public partial class Webcam
    {
        private readonly WebcamOptions _options = new WebcamOptions()
        {CanvasId = "canvas", VideoId = "video", PhotoId = "photo"};
        protected override void OnInitialized()
        {
            _options.Width = 320;
        }

        private async Task StartVideoAsync()
        {
            await _webcam.StartVideoAsync(_options);
        }

        private async Task TakePictureAsync()
        {
            await _webcam.TakePictureAsync();
        }
    }
}