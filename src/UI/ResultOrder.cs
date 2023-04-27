using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zenki.UI;

/// Data.Search мы будем использовать как быстрый невдумчиый поиск - чтобы быстро что-нибудь показать,
/// а этот уже будет более комплексный.
///
/// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1
/// https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.icomparer-1.compare#system-collections-generic-icomparer-1-compare(-0-0)
public sealed class ResultOrder : IComparer<string>
{
    private readonly string query; // то, что ввел пользователь в строке поиска

    public ResultOrder(string query)
    {
        this.query = query;
    }

    private static int GetMaxSubArray(string query, string str)
    {
        var max = 0;
        var curMax = 0;
        var queryCount = query.Length;
        
        for (var k = 0; k < queryCount; k++)
        {
            if (queryCount - k < max) break;
            var i = 0;
            var j = k;
            while (i < str.Length)
            {
                if (j >= queryCount) break;

                if (query[j] == str[i])
                {
                    curMax++;
                    i++;
                    j++;
                }

                else
                {
                    curMax = 0;
                    i++;
                    j = k;
                }

                max = Math.Max(max, curMax);
            }
        }

        return max;
    }

    private static int GetMaxSubsequence(string query, string str)
    {
        var max = 0;
        for (var k = 0; k < query.Length; k++)
        {
            if (query.Length - k <= max) break;
            
            var j = k;

            foreach (var c in str)
            {
                if (query[j] == c) j++;
            }

            max = Math.Max(max, j - k);
        }
        return max;
    }

    /// <summary>
    /// Нужно написать функцию которая опредяет какая из строк ближе к query.
    /// Т.е. с точки зрения этого компарера значение тем меньше, чем оно ближе к query.
    /// </summary>
    /// <returns>
    ///     -1, если x ближе к query, чем y
    ///      0, если строки с точки зрения удовлетворения query одинаковы
    ///      1, если y ближе к query, чем x
    /// </returns>
    public int Compare(string? x, string? y) // эти две строки в том числе выдал алгоритм поиска по триграммам 
    {
        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        if (x == y)
        {
            return 0;
        }


        var result = 0;
        
        // result = GetMaxSubArray(query, y)
        //     .CompareTo(GetMaxSubArray(query,x));

        if (result == 0)
        {
            result = GetMaxSubsequence(query, y)
                .CompareTo(GetMaxSubsequence(query, x));
        }
        
        var trigrams = Trigram.StringToTrigrams(query).ToList();
        var xTrigrams = Trigram.StringToTrigrams(x).ToList();
        var yTrigrams = Trigram.StringToTrigrams(y).ToList();

        if (result == 0)
        {
            result = yTrigrams.Intersect(trigrams).Count()
                .CompareTo(xTrigrams.Intersect(trigrams).Count());
        }

        if (result == 0)
        {
            result = y.Length.CompareTo(x.Length);
        }

        return result;
        

        // Попробуй разбить строки на триграммы и понять где триграммы query лучше сопадатют с триграммами x и y.
        // Например, идут в том же порядке или вовсе подряд. Поразмыщляй, может придумаешь еще какие-нибудь адекватные критерии. 
        const int dummy = 0;
        return dummy;
    }
}
