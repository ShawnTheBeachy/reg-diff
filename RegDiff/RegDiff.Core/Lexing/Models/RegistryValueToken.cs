namespace RegDiff.Core.Lexing.Models
{
    public sealed class RegistryValueToken : BaseToken
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
