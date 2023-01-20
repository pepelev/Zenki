using LightningDB;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Zenki.UI;

public sealed class LMDBTrigramIndex : ITrigramIndex<char, long>, IDisposable
{
    private readonly LightningEnvironment env;

    readonly string dbName = "trigrams";

    public LMDBTrigramIndex(string name)
    {
        env = new LightningEnvironment(name)
        {
            MaxDatabases = 2
        };

        env.Open();
        using var tx = env.BeginTransaction();
        using var db = tx.OpenDatabase(dbName, new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create | DatabaseOpenFlags.DuplicatesSort });
        tx.Commit();
    }

    public void Add((char, char, char) trigram, long value)
    {
        var (a, b, c) = trigram;
        using var tx = env.BeginTransaction();
        using var db = tx.OpenDatabase(dbName, new DatabaseConfiguration { Flags = DatabaseOpenFlags.DuplicatesSort });
        tx.Put(db, Encoding.UTF8.GetBytes($"{a}{b}{c}"), BitConverter.GetBytes(value), PutOptions.AppendDuplicateData);
        tx.Commit();
    }

    public void Dispose()
    {
        env.Dispose();
    }

    public IEnumerable<long> Search((char, char, char) trigram)
    {
        var (a, b, c) = trigram;
        using var tx = env.BeginTransaction(TransactionBeginFlags.ReadOnly);
        using var db = tx.OpenDatabase(dbName/*, new DatabaseConfiguration { Flags = DatabaseOpenFlags.DuplicatesSort }*/);
        using var cursor = tx.CreateCursor(db);
        var key = Encoding.UTF8.GetBytes($"{a}{b}{c}");

        cursor.Set(key);
        cursor.FirstDuplicate();

        while (true)
        {
            var output = BitConverter.ToInt64(cursor.GetCurrent().value.AsSpan());
            yield return output;
            if (cursor.NextDuplicate() != 0) break;
        }
    }
}
