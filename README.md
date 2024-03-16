# Query with dict

In your comment, you ask:

>Is there any way to get "ExtraProperties" column as Json in my query ? and then try to filter it ??

Given that `ExtraProperties` is a JSON string, you could make extension that does this by converting the string to a Dictionary<string, object> inline and using `Convert` on the value in the dictionary:

```
static partial class Extensions
{
    public static Dictionary<string, object> ToDict(this string json) =>
        JsonConvert
        .DeserializeObject<Dictionary<string, object>>(json ?? string.Empty)
        ?? new Dictionary<string, object>();
}
```
___

You could use it like this:

```
var newResult = query
    .Where(record => 
        Convert
        .ToBoolean(record.ExtraProperties.ToDict()["AutoGenerated"]));
```

___

Here's the console mock I used to test this answer.

[![selecting the property from the dictionary][1]][1]
```
using Newtonsoft.Json;

Console.Title = "Test ToDict() Extension";
// Create mock database. Casting ToArray()
// makes the random generation work correctly.
var mockDatabase =
    Enumerable.Range(0, 10)
    .Select(n => new MockItem { Id = n, })
    .ToArray();
// ==================================================

// Apples-to-apples IEnumerable
var query = mockDatabase.Where(_=>true);
Console.WriteLine($"Total item count = {query.Count()}");

query = query.Where(i =>
    Convert
    .ToBoolean(i.ExtraProperties.ToDict()["AutoGenerated"]));

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
    public static Dictionary<string, object> ToDict(this string json) =>
        JsonConvert
        .DeserializeObject<Dictionary<string, object>>(json ?? string.Empty)
        ?? new Dictionary<string, object>();
}
```


  [1]: https://i.stack.imgur.com/SHUyi.png