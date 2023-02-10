using System.Collections.Generic;
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

        // Попробуй разбить строки на триграммы и понять где триграммы query лучше сопадатют с триграммами x и y.
        // Например, идут в том же порядке или вовсе подряд. Поразмыщляй, может придумаешь еще какие-нибудь адекватные критерии. 
        const int dummy = 0;
        return dummy;
    }
}