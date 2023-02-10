using System.Text;
using NUnit.Framework;

namespace Zenki.Tests;

public sealed class Experiments
{
    [Test]
    public void Runes_From_Emoji()
    {
        const string str = @"⏳⏳⚡";
        var utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var preambule = utf8Encoding.Preamble.ToArray();
        var bytes1 = preambule.Concat(utf8Encoding.GetBytes(str)).ToArray().AsSpan();
        var bytes = Encoding.UTF8.GetBytes(str).AsSpan();
        for (var i = 0; i <= bytes1.Length; i++)
        {
            var status = Rune.DecodeFromUtf8(bytes1[i..], out var rune, out var bytesConsumed);
        }
    }
}