using RegDiff.Core;
using RegDiff.Core.Lexing;
using RegDiff.Core.Lexing.Sources;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace RegDiff.UWP.ViewModels
{
    public sealed class RegistryTreeViewModel : BindableBase
    {
        #region Data

        private string[][] _data;
        public string[][] Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        #endregion Data

        #region File

        private StorageFile _file;
        public StorageFile File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        #endregion File

        #region Progress

        private ProcessingProgressViewModel _progress;
        public ProcessingProgressViewModel Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        #endregion Progress

        public async Task ParseAsync(string text,
                                     IProgress<ProgressIncrement> progress)
        {
            var lexer = new Lexer(new StringSource(text));
            Data = await lexer.LexAsync(progress);
        }
    }
}
