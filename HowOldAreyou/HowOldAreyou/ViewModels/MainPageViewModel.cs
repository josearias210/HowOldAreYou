namespace HowOldAreyou.ViewModels
{
    using HowOldAreyou.Services;
    using Plugin.Media;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;
    public class MainPageViewModel : ViewModelBase
    {
        #region  Attributes

        private int age;
        private bool isBusy;
        private decimal score;
        private string pathImage;
        private bool isLoadPhoto;
        private ImageSource photo;

        #endregion

        #region Properties

        public ImageSource Photo
        {
            get { return photo; }
            set
            {
                photo = value;
                OnPropertyChanged();
            }
        }
        public bool IsLoadPhoto
        {
            get { return isLoadPhoto; }
            set
            {
                isLoadPhoto = value;
                OnPropertyChanged();
            }
        }
        public int Age
        {
            get { return age; }
            set
            {
                age = value;
                OnPropertyChanged();
            }
        }
        public decimal Score
        {
            get { return score; }
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Service

        private FaceService service;

        #endregion

        #region Commands

        public ICommand TakePhotoCommand { get; }
        public ICommand PickPhotoCommand { get; }
        public ICommand AnalyzeCommand { get; }

        #endregion

        #region Constructs
        public MainPageViewModel()
        {
            service = new FaceService();
            PickPhotoCommand = new Command(async () => await PickPhoto());
            TakePhotoCommand = new Command(async () => await TakePhoto());
            AnalyzeCommand = new Command(async () => await Analyze());
        }


        #endregion

        #region Private

        private async Task Analyze()
        {
            if (Photo == null || string.IsNullOrEmpty(pathImage) || pathImage == "default.jpg")
            {
                await Application.Current.MainPage.DisplayAlert("Xamarin Latino", "No has seleccionado una foto", "Ok");
                return;
            }

            IsBusy = true;
            try
            {
                var age = await service.DetectAge(pathImage);
                if (age > 0)
                {
                    this.Age = (int)age;
                }

            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Xamarin Latino", "No se pudo evaluar la foto", "Ok");
            }
            IsBusy = false;

        }

        private async Task PickPhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
                return;


            Photo = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                pathImage = file.Path;
                IsLoadPhoto = true;
                return stream;
            });
        }

        private async Task TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "XamarinLatino",
                Name = "age.jpg"
            });

            if (file == null)
                return;


            Photo = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                pathImage = file.Path;
                IsLoadPhoto = true;
                return stream;
            });
        }
        #endregion

    }
}
