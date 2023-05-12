using FluentAssertions;
using NUnit.Framework;
using Zenki.UI;
using File = Zenki.UI.File;

namespace Zenki.Tests;

// общий алгоритм такой:
// 1. читаем начало файла и пробуем подгадать кодировку (не обязательно ограничиваться одним вариантом) обработать BOM
// 2. читаем файл с конца до начала, т.к. обычно интересны актуальные логи
// 3. определяем границы log-entry 
// 4. индексируем триграммы
// 5. определяем свойства log-entry
// 6. индексируем свойства
//
// Пока релизовано так, чтобы хоть что-то работало
public sealed class LogFileShould
{
    [Test]
    [Explicit]
    public void Example()
    {
        const string logPath = @"../../log-examples/structure.log";
        using var file = new File(
            new FileStream(
                logPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                0
            )
        );
        var log = new LogFile(file);
        log.ElementAt(2).Entry.Properties.Should().Contain(("Severity", "DBG"));
    }
}