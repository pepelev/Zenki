using System;
using System.Collections.Generic;
using System.Text;
using LightningDB;

namespace Zenki.UI;

public sealed class LmdbTrigramIndex : ITrigramIndex<char, long>, IDisposable
{
    private readonly string dbName = "trigrams";
    private readonly LightningEnvironment env;

    public LmdbTrigramIndex(string name)
    {
        env = new LightningEnvironment(name)
        {
            MaxDatabases = 2
        };

        env.Open();
        using var tx = env.BeginTransaction();
        using var db = tx.OpenDatabase(
            dbName,
            new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create | DatabaseOpenFlags.DuplicatesSort }
        );
        CheckSuccess(tx.Commit());
    }

    public void Dispose() => env.Dispose();

    public void Add((char, char, char) trigram, long value)
    {
        var (a, b, c) = trigram;
        using var tx = env.BeginTransaction();
        using var db = tx.OpenDatabase(dbName, new DatabaseConfiguration { Flags = DatabaseOpenFlags.DuplicatesSort });
        CheckSuccess(
            tx.Put(
                db,
                Encoding.UTF8.GetBytes($"{a}{b}{c}"),
                BitConverter.GetBytes(value),
                PutOptions.AppendDuplicateData
            )
        );
        CheckSuccess(tx.Commit());
    }

    public IEnumerable<long> Search((char, char, char) trigram)
    {
        var (a, b, c) = trigram;
        using var tx = env.BeginTransaction(TransactionBeginFlags.ReadOnly);
        using var db = tx.OpenDatabase(dbName);
        using var cursor = tx.CreateCursor(db);
        var key = Encoding.UTF8.GetBytes($"{a}{b}{c}");

        var keyStatus = cursor.Set(key);
        if (keyStatus != MDBResultCode.Success)
        {
            yield break;
        }

        cursor.FirstDuplicate();

        while (true)
        {
            var output = BitConverter.ToInt64(cursor.GetCurrent().value.AsSpan());
            yield return output;

            if (cursor.NextDuplicate() != MDBResultCode.Success)
            {
                break;
            }
        }
    }

    private static void CheckSuccess(MDBResultCode result)
    {
        if (result == MDBResultCode.Success)
        {
            return;
        }

        throw new Exception("Transaction unsuccessful");
    }
}