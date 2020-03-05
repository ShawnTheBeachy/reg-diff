using RegDiff.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RegDiff.UWP.Views
{
    public sealed class RegistryItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate KeyTemplate { get; set; }
        public DataTemplate ValueTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is RegistryKey)
            {
                return KeyTemplate;
            }

            else if (item is BaseRegistryValue)
            {
                return ValueTemplate;
            }

            else
            {
                return null;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
