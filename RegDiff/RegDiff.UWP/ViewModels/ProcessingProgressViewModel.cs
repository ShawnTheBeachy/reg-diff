using RegDiff.Core;
using System;

namespace RegDiff.UWP.ViewModels
{
    public sealed class ProcessingProgressViewModel : BindableBase
    {
        #region Current

        private int _current;
        public int Current
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        #endregion Current

        #region Message

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        #endregion Message

        #region Percent

        private int _percent;
        public int Percent
        {
            get => _percent;
            set => SetProperty(ref _percent, value);
        }

        #endregion Percent

        #region Total

        private int _total;
        public int Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        #endregion Total

        public ProcessingProgressViewModel(Progress<ProgressIncrement> progress)
        {
            progress.ProgressChanged += Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object sender, ProgressIncrement e)
        {
            Current = e.Current;
            Message = e.Message;
            Percent = (int)Math.Round((double)Current / Total * 100);
            Total = e.Total;
        }
    }
}
