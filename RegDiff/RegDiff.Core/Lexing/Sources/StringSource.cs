namespace RegDiff.Core.Lexing.Sources
{
    public sealed class StringSource : ISource
    {
        private readonly string _raw;

        public StringSource(string input)
        {
            _raw = input;
        }

        public string GetRaw()
        {
            return _raw;
        }
    }
}
