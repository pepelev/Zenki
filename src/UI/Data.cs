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

            // newIndex.linesCount = lines.Length; //new
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
        if (head.Length == 0)
        {
            return Enumerable.Empty<string>();
        }

        var result = new List<string>();
        
        foreach (var entry in head)
        {
            result.AddRange(index.Search(entry).Select(e => e.Line));
        }
        
        return result.ToHashSet();
    }

    public string? Found { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
}
