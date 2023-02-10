using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Zenki.UI;

public sealed class Data : INotifyPropertyChanged
{
    private bool useResultOrder = false;
    private string filePath = "";
    private string query = "";

    private TrigramIndex<Rune, (long Offset, string Line)>? index;

    public Data()
    {
        FilePath = @"../../log-examples/structure.log";
    }

    public string FilePath
    {
        get => filePath;
        set
        {
            if (value == filePath)
            {
                return;
            }

            if (!System.IO.File.Exists(value))
            {
                filePath = value;
                return;
            }

            using var file = new File(
                new FileStream(
                    value,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete,
                    0
                )
            );
            var newIndex = new TrigramIndex<Rune, (long Offset, string Line)>();
            var valueTuples = new LogFile(file).ToList();
            foreach (var (offset, entry) in valueTuples)
            {
                var trigrams = Trigram.StringToTrigrams(entry.Raw);
                foreach (var trigram in trigrams)
                {
                    newIndex.Add(trigram, (offset, entry.Raw));
                }
            }

            (index, filePath) = (newIndex, value);
        }
    }

    public bool UseResultOrder
    {
        get => useResultOrder;
        set
        {
            if (value != useResultOrder)
            {
                useResultOrder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseResultOrder)));
            }
        }
    }
    
    public string Query
    {
        get => query;
        set
        {
            if (value == query)
            {
                return;
            }

            if (index == null)
            {
                query = value;
                return;
            }

            const int threshold = 1000;
            var list = Search(value, index).Take(threshold + 1).ToList();
            if (list.Count <= threshold)
            {
                var order = new ResultOrder(value);
                list.Sort(order);
                UseResultOrder = true;
            }
            else
            {
                list = list.Take(25).ToList();
                UseResultOrder = false;
            }

            Found = string.Join("\n", list);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Found)));
            query = value;
        }
    }

    private static IEnumerable<string> Search(string query, ITrigramIndex<Rune, (long Offset, string Line)> index)
    {
        var trigrams = Trigram.StringToTrigrams(query).ToList();

        var lines = new Dictionary<long, string>();
        var weights = new Dictionary<long, double>();

        foreach (var linesContainingTrigram in trigrams.Select(trigram => index.Search(trigram).ToList()))
        {
            foreach (var (i, line) in linesContainingTrigram)
            {
                lines[i] = line;
                var weight = 1.0 / (1 + Math.Log(linesContainingTrigram.Count));
                if (weights.ContainsKey(i))
                    weights[i] += weight;
                else
                    weights[i] = weight;
            }
        }

        return lines.Count == 0
            ? Enumerable.Empty<string>()
            : weights
                .OrderByDescending(x => x.Value)
                .Select(x => lines[x.Key]);
    }

    public string? Found { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
}
