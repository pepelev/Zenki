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
        var head = Trigram.StringToTrigrams(query).ToArray();

        var lines = new Dictionary<int, string>();
        var weights = new Dictionary<int, double>();

        foreach (var trigram in head)
        {
            var linesContainingTrigram = index.Search(trigram).ToList();
            foreach (var (i, line) in linesContainingTrigram)
            {
                lines[i] = line;
                var w = 1.0 / (1 + Math.Log(linesContainingTrigram.Count));
                if (weights.ContainsKey(i))
                    weights[i] += w;
                else
                    weights[i] = w;
            }
        }

        return lines.Count == 0
            ? Enumerable.Empty<string>()
            : weights
                .OrderByDescending(x => x.Value)
                .Select(x => lines[x.Key]);
        
        // return head.Length == 0
        //     ? Enumerable.Empty<string>()
        //     : head
        //         .Select(entry => index.Search(entry).ToList())
        //         .OrderBy(e => e.Count)
        //         .SelectMany(e => e)
        //         .Select(e => e.Line)
        //         .Distinct();
    }

    public string? Found { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
}
