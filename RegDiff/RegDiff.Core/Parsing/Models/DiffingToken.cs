using RegDiff.Core.Lexing.Models;

namespace RegDiff.Core.Parsing.Models
{
    public sealed class DiffingToken
    {
        public bool IsInA { get; set; }
        public bool IsInB { get; set; }
        public string[] Token { get; set; }
    }
}
