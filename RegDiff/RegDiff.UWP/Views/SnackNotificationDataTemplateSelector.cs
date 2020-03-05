using RegDiff.UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RegDiff.UWP.Views
{
    public sealed class SnackNotificationDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HidNodeTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is string)
            {
                return StringTemplate;
            }

            if (item is UndoHideItemModel)
            {
                return HidNodeTemplate;
            }

            return base.SelectTemplateCore(item);
        }
    }
}
