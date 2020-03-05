using RegDiff.Core.Models;
using RegDiff.Core.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegDiff.Core.Parsing
{
    public sealed class Parser
    {
        public static int BinarySearch(string[][] name, string item)
        {
            var min = 0;
            var N = name.Length;
            var max = N - 1;

            do
            {
                var mid = (min + max) / 2;

                if (string.Compare(item, name[mid][0]) > 0)
                {
                    min = mid + 1;
                }

                else
                {
                    max = mid - 1;
                }

                if (string.Compare(item, name[mid][0]) == 0)
                {
                    return mid;
                }
            } while (min <= max);

            return -1;
        }

        public async Task<IList<RegistryKey>> ParseAsync(string[][] a,
                                                         string[][] b,
                                                         IProgress<ProgressIncrement> progress)
        {
            var keys = new List<RegistryKey>();

            a = a.OrderBy(x => x[0]).ToArray();
            b = b.OrderBy(x => x[0]).ToArray();

            var checkedB = new bool[b.Length];
            var inANotInB = new List<DiffingToken>();
            var inBNotInA = new List<DiffingToken>();
            var inBoth = new List<DiffingToken>();
            var total = a.Length + (int)Math.Round(b.Length * 1.25);
            var j = 0;

            for (var i = 0; i < a.Length; i++)
            {
                var matchIndex = BinarySearch(b, a[i][0]);

                if (matchIndex < 0)
                {
                    inANotInB.Add(new DiffingToken
                    {
                        IsInA = true,
                        IsInB = false,
                        Token = a[i]
                    });
                }

                else
                {
                    checkedB[matchIndex] = true;
                    inBoth.Add(new DiffingToken
                    {
                        IsInA = true,
                        IsInB = true,
                        Token = a[i]
                    });
                }

                if (i % 4973 == 0)
                {
                    await Task.Run(() => progress.Report(new ProgressIncrement
                    {
                        Current = i,
                        Message = $"Diffing files... ({i} of {total})",
                        Total = total
                    }));
                }

                j = i;
            }

            for (var i = 0; i < b.Length; i++)
            {
                if (checkedB[i])
                {
                    continue;
                }

                var matchIndex = BinarySearch(a, b[i][0]);

                if (matchIndex < 0)
                {
                    inBNotInA.Add(new DiffingToken
                    {
                        IsInA = false,
                        IsInB = true,
                        Token = b[i]
                    });
                }


                if ((j + i) % 4973 == 0)
                {
                    await Task.Run(() => progress.Report(new ProgressIncrement
                    {
                        Current = j + i,
                        Message = $"Diffing files... ({j + i} of {total})",
                        Total = total
                    }));
                }
            }

            var combined = inBoth
                .Concat(inANotInB)
                .Concat(inBNotInA)
                .OrderBy(x => x.Token[0])
                .ToArray();

            for (var i = 0; i < combined.Length; i++)
            {
                var token = combined[i];

                if (token.Token.Length == 1)
                {
                    var key = new RegistryKey
                    {
                        IsInA = token.IsInA,
                        IsInB = token.IsInB
                    };
                    i = await ParseKeyAsync(token, combined, i + 1, key, progress);

                    if (i % 3726 == 0)
                    {
                        var progressIncrement = new ProgressIncrement
                        {
                            Current = i + 1,
                            Message = $"Building view... ({i + 1} of {combined.Length})",
                            Total = combined.Length
                        };
                        await Task.Run(() => progress?.Report(progressIncrement));
                    }

                    keys.Add(key);
                }
            }

            await Task.Run(() => progress?.Report(new ProgressIncrement
            {
                Current = combined.Length,
                Message = "Finished diffing result.",
                Total = combined.Length
            }));
            return keys;
        }

        private static async Task<int> ParseKeyAsync(DiffingToken token,
                                                     DiffingToken[] tokens,
                                                     int startIndex,
                                                     RegistryKey key,
                                                     IProgress<ProgressIncrement> progress,
                                                     RegistryKey parent = null)
        {
            bool ShouldContinue(int index)
            {
                return index < tokens.Length && tokens[index].Token[0].StartsWith(token.Token[0]);
            }

            key.Path = token.Token[0];
            key.Name = token.Token[0].Split('\\').Last();
            key.HasDifference = !key.IsInB || !key.IsInA;

            if (key.HasDifference && parent != null)
            {
                parent.HasDifference = true;
            }

            for (; ShouldContinue(startIndex); startIndex++)
            {
                var nextToken = tokens[startIndex];

                // Registry key.
                if (nextToken.Token.Length == 1)
                {
                    var nextKey = new RegistryKey
                    {
                        IsInA = nextToken.IsInA,
                        IsInB = nextToken.IsInB
                    };
                    startIndex = await ParseKeyAsync(nextToken, tokens, startIndex + 1, nextKey, progress, key);

                    if (key.HasDifference && parent != null)
                    {
                        parent.HasDifference = true;
                    }

                    key.Keys.Add(nextKey);
                }

                // Registry value.
                else if (nextToken.Token.Length == 3)
                {
                    key.Values.Add(ParseValue(nextToken, key));

                    if (key.HasDifference && parent != null)
                    {
                        parent.HasDifference = true;
                    }
                }

                if (startIndex + 1 % 3726 == 0)
                {
                    var progressIncrement = new ProgressIncrement
                    {
                        Current = startIndex + 1,
                        Message = $"Building view... ({startIndex + 1} of {tokens.Length})",
                        Total = tokens.Length
                    };
                    await Task.Run(() => progress?.Report(progressIncrement));
                }
            }

            return --startIndex;
        }

        private static BaseRegistryValue ParseValue(DiffingToken token,
                                                    RegistryKey parent = null)
        {
            BaseRegistryValue value;

            // TODO
            value = new RegistryString { IsVisible = true, Name = token.Token[1], Value = token.Token[2] };

            //switch (valueToken.Type)
            //{
            //    case RegistryValueType.REG_SZ:
            //        value = new RegistryString
            //        {
            //            Name = valueToken.Name,
            //            Value = valueToken.Value
            //        };
            //        break;
            //    case RegistryValueType.REG_DWORD:
            //        value = new RegistryDWORD
            //        {
            //            Name = valueToken.Name,
            //            Value = valueToken.Value
            //        };
            //        break;
            //    default:
            //        value = new RegistryString
            //        {
            //            Name = valueToken.Name
            //        };
            //        break;
            //}

            value.IsInA = token.IsInA;
            value.IsInB = token.IsInB;
            value.HasDifference = !value.IsInB || !value.IsInA;

            if (value.HasDifference && parent != null)
            {
                parent.HasDifference = true;
            }

            return value;
        }
    }
}
