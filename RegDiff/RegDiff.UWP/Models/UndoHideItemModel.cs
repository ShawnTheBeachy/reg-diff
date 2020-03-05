using Prism.Commands;
using RegDiff.Core.Models;

namespace RegDiff.UWP.Models
{
    public sealed class UndoHideItemModel
    {
        public BaseRegistryModel Item { get; set; }
        public DelegateCommand UndoCommand { get; set; }

        public UndoHideItemModel()
        {
            UndoCommand = new DelegateCommand(() => Item.IsVisible = true);
        }
    }
}
