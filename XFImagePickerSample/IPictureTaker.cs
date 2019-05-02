using System.IO;
using System.Threading.Tasks;

namespace XFImagePickerSample
{
    public interface IPictureTaker
    {
        Task<Stream> TakePicture();
        void Dispose();
    }
}
