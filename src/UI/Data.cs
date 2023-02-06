using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Zenki.UI;

public sealed class Data : INotifyPropertyChanged
{
    private string filePath = @"C:\Users";
    // private string filePath = @"E:\input\system.log";
    private string query = "";
    private TrigramIndex<Rune, (int Index, string Line)>? index;

    public string FilePath
    {
        get => filePath;
        set
        {
            if (value == filePath)
            {
                return;
            }

            if (!File.Exists(value))
            {
                filePath = value;
                return;
            }

            var lines = File.ReadAllLines(value, Encoding.UTF8);
            var newIndex = new TrigramIndex<Rune, (int Index, string Line)>();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trigrams = Trigram.StringToTrigrams(line);
                foreach (var trigram in trigrams)
                {
                    newIndex.Add(trigram, (i, line));
                }
            }

            (index, filePath) = (newIndex, value);
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

            var list = Search(value, index).Take(20);
            Found = string.Join("\n", list);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Found)));
            query = value;
        }
    }

    private static IEnumerable<string> Search(string query, ITrigramIndex<Rune, (int Index, string Line)> index)
    {
        var trigrams = Trigram.StringToTrigrams(query).ToList();

        var lines = new Dictionary<int, string>();
        var weights = new Dictionary<int, double>();

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
