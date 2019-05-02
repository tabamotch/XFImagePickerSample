using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XFImagePickerSample.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ICommand TakePictureButtonCommand { get; }

        private ImageSource _imgSource;
        public ImageSource Img
        {
            get => _imgSource;
            set => SetProperty(ref _imgSource, value);
        }

        public MainPageViewModel()
        {
            TakePictureButtonCommand = new Command(async () => await TakePicture());
        }

        private async Task TakePicture()
        {
            IPictureTaker pictureTaker = null;
            Stream stream = null;
            try
            {
                pictureTaker = DependencyService.Get<IPictureTaker>(DependencyFetchTarget.NewInstance);
                stream = await pictureTaker.TakePicture();

                if(stream != null)
                {
                    Img = ImageSource.FromStream(() => stream);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                stream?.Dispose();
                stream = null;

                pictureTaker?.Dispose();
                pictureTaker = null;
            }
        }

        private void SetProperty<T>(ref T initial, T newValue, [CallerMemberName]string propertyName = null)
        {
            if(!initial.Equals(newValue))
            {
                initial = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
