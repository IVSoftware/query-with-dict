﻿using Newtonsoft.Json;

Console.Title = "Test ToDict() Extension";
var query =
    Enumerable.Range(0, 10)
    .Select(n => new MockItem { Id = n, });

Console.WriteLine($"Total item count = {query.Count()}");

query = query
    .Where(i => Convert.ToBoolean(i.ExtraProperties.ToDict()["AutoGenerated"]));

Console.WriteLine($"Reduced item count = {query.Count()}");
Console.WriteLine(string.Join(Environment.NewLine, query.Select(record=>record.ToString())));
Console.ReadLine();


class MockItem
{
    static Random _rando = new Random(1);
    public  MockItem() 
    {
        // Generate random T/F for property and
        // save as json formatted string.
        ExtraProperties = JsonConvert.SerializeObject(
            new Dictionary<string, object>
            {
                { "AutoGenerated", _rando.Next(2).Equals(1) },
            });
    }

    public int Id { get; set; }
    public string ExtraProperties { get; }
    public override string ToString() =>
        $"{Id} ExtraProperties = {ExtraProperties}";
}

static partial class Extensions
{
    public static Dictionary<string, string> ToDict(this string json) =>
        JsonConvert
        .DeserializeObject<Dictionary<string, string>>(json ?? string.Empty)
        ?? new Dictionary<string, string>();
}