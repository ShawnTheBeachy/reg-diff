namespace RegDiff.Core.Models
{
    public abstract class BaseRegistryModel : BindableBase
    {
        public bool HasDifference { get; set; }
        public bool IsInA { get; set; }
        public bool IsInB { get; set; }

        #region IsVisible

        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        #endregion IsVisible

        public string Name { get; set; }
        public string Path { get; set; }
    }
}
