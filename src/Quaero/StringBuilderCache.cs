// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// Based on https://github.com/dotnet/runtime/blob/bdd12cc21cd9bc6c46b143917db39c381edb9c0e/src/libraries/Common/src/System/Text/StringBuilderCache.cs.

using System.Text;

namespace Quaero;

/// <summary>
/// Provide a cached reusable instance of stringbuilder per thread.
/// </summary>
internal static class StringBuilderCache
{
    // The value 360 was chosen in discussion with performance experts as a compromise between using
    // as little memory per thread as possible and still covering a large part of short-lived
    // StringBuilder creations on the startup path of VS designers.
    private const int MaxBuilderSize = 360;

    private const int DefaultCapacity = 16; // == StringBuilder.DefaultCapacity

    [ThreadStatic]
    private static StringBuilder? _cachedInstance;

    /// <summary>
    /// Get a StringBuilder for the specified capacity.
    /// </summary>
    /// <remarks>
    /// If a StringBuilder of an appropriate size is cached, it will be returned and the cache emptied.
    /// </remarks>
    public static StringBuilder Acquire(int capacity = DefaultCapacity)
    {
        if (capacity > MaxBuilderSize)
        {
            return new StringBuilder(capacity);
        }

        var builder = _cachedInstance;
        if (builder is null)
        {
            return new StringBuilder(capacity);
        }

        // Avoid stringbuilder block fragmentation by getting a new StringBuilder
        // when the requested size is larger than the current capacity
        if (capacity > builder.Capacity)
        {
            return new StringBuilder(capacity);
        }

        _cachedInstance = null;
        builder.Clear();
        return builder;
    }

    /// <summary>
    /// Place the specified builder in the cache if it is not too big.
    /// </summary>
    public static void Release(StringBuilder builder)
    {
        if (builder.Capacity <= MaxBuilderSize)
        {
            _cachedInstance = builder;
        }
    }

    /// <summary>
    /// ToString() the stringbuilder, Release it to the cache, and return the resulting string.
    /// </summary>
    public static string GetStringAndRelease(StringBuilder builder)
    {
        var result = builder.ToString();
        Release(builder);
        return result;
    }
}
