if (args.Length > 0 && args[0].Equals("task008", StringComparison.OrdinalIgnoreCase))
{
    await Base.Exercises.Task008.Task008Exercise.RunAsync();
}
else if (args.Length > 0 && args[0].Equals("task007", StringComparison.OrdinalIgnoreCase))
{
    await Base.Exercises.Task007.Task007Exercise.RunAsync(args[1..]);
}
else
{
    await Base.Exercises.Task006.Task006Exercise.RunAsync(args);
}

