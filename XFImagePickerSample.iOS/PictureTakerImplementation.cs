using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(XFImagePickerSample.iOS.PictureTakerImplementation))]

namespace XFImagePickerSample.iOS
{
    // 参考にしたページ：https://docs.microsoft.com/ja-jp/xamarin/xamarin-forms/app-fundamentals/dependency-service/photo-picker
    // 参考にしたページ：https://stackoverflow.com/questions/32446520/how-to-take-a-picture-with-ios-camera-and-save-the-image-to-camera-roll-using-xa

    public class PictureTakerImplementation : IPictureTaker, IDisposable
    {
        TaskCompletionSource<Stream> _taskCompletionSource;
        UIImagePickerController _controller;

        public Task<Stream> TakePicture()
        {
            if(_controller == null)
            {
                _controller = new UIImagePickerController
                {
                    SourceType = UIImagePickerControllerSourceType.Camera,
                    MediaTypes = new[] { "public.image" },
                    AllowsEditing = false,

                    // ↓をfalseにすると、シャッターボタンまで表示されなくなる
                    //ShowsCameraControls = false
                };

                //_controller.PrefersStatusBarHidden();
                //_controller.CameraOverlayView = new CameraOverlayView();
                var cameraOverlayView = _controller.CameraOverlayView;

                _controller.FinishedPickingImage += _controller_FinishedPickingImage;
                _controller.Canceled += _controller_Canceled;
            }

            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentViewController(_controller, true, () => { });

            _taskCompletionSource = new TaskCompletionSource<Stream>();
            return _taskCompletionSource.Task;
        }

        private void _controller_FinishedPickingImage(object sender, UIImagePickerImagePickedEventArgs args)
        {
            UIImage image = args.EditingInfo[UIImagePickerController.EditedImage] as UIImage;
            if(image == null)
            {
                image = args.Image;
            }

            UnRegisterEventHandler();
            if (image != null)
            {
                NSData data = image.AsPNG();
                Stream stream = data.AsStream();

                _taskCompletionSource.SetResult(stream);
            }
            else
            {
                _taskCompletionSource.SetResult(null);
            }

            _controller.DismissModalViewController(true);
            //_controller.Dispose();
            //_controller = null;
        }

        private void _controller_Canceled(object sender, EventArgs e)
        {
            UnRegisterEventHandler();
            _taskCompletionSource.SetResult(null);
            _controller.DismissModalViewController(true);
            //_controller.Dispose();
            //_controller = null;
        }

        void UnRegisterEventHandler()
        {
            _controller.FinishedPickingImage -= _controller_FinishedPickingImage;
            _controller.Canceled -= _controller_Canceled;
        }

        public void Dispose()
        {

        }
    }
}
