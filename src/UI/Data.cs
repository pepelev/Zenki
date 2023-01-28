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
    private TrigramIndex<char, (int Index, string Line)>? index;

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
            var newIndex = new TrigramIndex<char, (int Index, string Line)>();
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

    private static IEnumerable<string> Search(string query, ITrigramIndex<char, (int Index, string Line)> index)
    {
        var head = Trigram.StringToTrigrams(query).ToArray();

        var weights = new List<(string Line, double Weight)>();

        foreach (var trigram in head)
        {
            var searchResult = index.Search(trigram).Select(e=> e.Line).ToList();
            var linesCount = searchResult.Count;
            
            weights.AddRange(searchResult.Select((entry, i) => (entry, Math.Log(linesCount - i + 1) / linesCount)));
        }

        return head.Length == 0
            ? Enumerable.Empty<string>()
            : weights
                .OrderByDescending(e => e.Weight)
                .Select(e => e.Line)
                .Distinct();
        
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
