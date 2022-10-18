﻿namespace Azure.EventGrid.Simulator.Extensions;

public static class CollectionExtensions
{
    public static bool HasItems<T>(this ICollection<T> collection)
    {
        return collection != null && collection.Any();
    }

    public static string Separate<T>(this ICollection<T> collection, string separator = ", ", Func<T, string> toStringFunction = null)
    {
        toStringFunction ??= t => t.ToString();

        return string.Join(separator, (collection ?? Array.Empty<T>()).Select(c => toStringFunction(c)));
    }
}