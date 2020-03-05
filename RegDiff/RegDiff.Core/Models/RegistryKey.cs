using System.Collections.Generic;
using System.Linq;

namespace RegDiff.Core.Models
{
    public sealed class RegistryKey : BaseRegistryModel
    {
        public IList<BaseRegistryModel> Children => 
            Keys.Concat<BaseRegistryModel>(Values).ToList();

        public IList<RegistryKey> Keys { get; set; }
        public RegistryKey Parent { get; set; }
        public IList<BaseRegistryValue> Values { get; set; }

        public RegistryKey()
        {
            IsVisible = true;
            Keys = new List<RegistryKey>();
            Values = new List<BaseRegistryValue>();
        }

        public void DoesHaveDifference()
        {
            HasDifference = true;
            Parent?.DoesHaveDifference();
        }
    }
}
