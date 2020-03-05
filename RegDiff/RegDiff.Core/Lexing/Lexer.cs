using RegDiff.Core.Lexing.Sources;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegDiff.Core.Lexing
{
    public sealed class Lexer
    {
        private const string SLICE_ONE_PATTERN = "^[\\t ]*(?'key'\\[[^]]*\\])(?'value'[^[]*)";
        private const string SLICE_TWO_PATTERN = "^[\\t ]*(?'name'\".+\"|@)=(?'value'\"(?:[^\"\\\\]|\\\\.)*\"|[^\"]+)";

        private readonly ISource _source;

        public Lexer(ISource source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public async Task<string[][]> LexAsync(IProgress<ProgressIncrement> progress)
        {
            await Task.Run(() => progress.Report(new ProgressIncrement
            {
                Current = 0,
                Message = "Preparing to lex file...",
                Total = 100
            }));

            var tokens = new List<string[]>();
            var matches = Regex.Matches(_source.GetRaw(), SLICE_ONE_PATTERN, RegexOptions.Multiline | RegexOptions.Singleline);
            
            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var key = match.Groups["key"].Value.Trim('\r', '\n').TrimStart('[').TrimEnd(']');
                var value = match.Groups["value"].Value.Trim('\r', '\n');
                tokens.Add(new[] { key });

                var valueMatches = Regex.Matches(value, SLICE_TWO_PATTERN, RegexOptions.Multiline);

                foreach (Match valueMatch in valueMatches)
                {
                    var valueName = valueMatch.Groups["name"].Value.Trim('\r', '\n').Trim('"');
                    var valueValue = valueMatch.Groups["value"].Value.Trim('\r', '\n').Trim('"');
                    tokens.Add(new[] { $"{key}\\{valueName}", valueName, valueValue });
                }

                if (i % 2736 == 0)
                {
                    await Task.Run(() => progress.Report(new ProgressIncrement
                    {
                        Current = i,
                        Message = $"Lexing file ({i} of {matches.Count})",
                        Total = matches.Count
                    }));
                }
            }

            await Task.Run(() => progress.Report(new ProgressIncrement
            {
                Current = 1,
                Message = "Finished lexing file.",
                Total = 1
            }));
            return tokens.ToArray();
        }
    }
}
