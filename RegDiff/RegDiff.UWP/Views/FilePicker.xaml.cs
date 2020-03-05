using RegDiff.Core;
using RegDiff.UWP.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RegDiff.UWP.Views
{
    public sealed partial class FilePicker : UserControl
    {
        #region DataContext

        public new RegistryTreeViewModel DataContext
        {
            get => (RegistryTreeViewModel)GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        public new static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(RegistryTreeViewModel), typeof(FilePicker), new PropertyMetadata(null));

        #endregion DataContext

        #region Placeholder

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(FilePicker), new PropertyMetadata("File"));

        #endregion Placeholder

        public event RoutedEventHandler FileProcessingEnded;
        public event RoutedEventHandler FileProcessingStarted;

        public FilePicker()
        {
            InitializeComponent();

            FileProcessingEnded += FilePicker_FileProcessingEnded;
            FileProcessingStarted += FilePicker_FileProcessingStarted;
        }

        private async void FileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext.File = await PickFileAsync();
            await ProcessFileAsync(DataContext);
        }

        private void FilePicker_FileProcessingStarted(object sender, RoutedEventArgs e)
        {
            StartFileProcessingAnimations();
        }

        private void FilePicker_FileProcessingEnded(object sender, RoutedEventArgs e)
        {
            StartFileProcessingDoneAnimations();
        }

        private async Task<StorageFile> PickFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".reg");
            var file = await picker.PickSingleFileAsync();
            return file;
        }

        private async Task ProcessFileAsync(RegistryTreeViewModel viewModel)
        {
            if (viewModel.File == null)
            {
                return;
            }

            FileProcessingStarted?.Invoke(this, new RoutedEventArgs());

            var progress = new Progress<ProgressIncrement>();
            viewModel.Progress = new ProcessingProgressViewModel(progress);
            await Task.Run(() =>
            {
                (progress as IProgress<ProgressIncrement>).Report(new ProgressIncrement
                {
                    Current = 0,
                    Message = "Preparing...",
                    Total = 100
                });
            });

            var text = await FileIO.ReadTextAsync(viewModel.File);
            await viewModel.ParseAsync(text, progress);
            FileProcessingEnded?.Invoke(this, new RoutedEventArgs());
        }

        private void StartFileProcessingAnimations()
        {
            FileProcessingProgressFade.StartAnimation();
        }

        private void StartFileProcessingDoneAnimations()
        {
            FileProcessingDoneProgressFade.StartAnimation();
        }
    }
}
