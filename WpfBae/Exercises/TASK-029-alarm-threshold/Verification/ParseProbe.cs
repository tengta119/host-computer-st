using System;
foreach (string text in new string[] { "26.5", "abc", "NaN" })
{
    try
    {
        double value = double.Parse(text);
        Console.WriteLine(text + ": Parse succeeded; IsNaN=" + double.IsNaN(value) + "; <20=" + (value < 20) + "; >30=" + (value > 30));
    }
    catch (FormatException)
    {
        Console.WriteLine(text + ": caught FormatException");
    }
}
double result;
bool success = double.TryParse("NaN", out result);
Console.WriteLine("TryParse NaN: success=" + success + "; IsNaN=" + double.IsNaN(result));