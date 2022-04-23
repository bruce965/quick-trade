// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/util/raw/master/LICENSE

using System.Diagnostics.CodeAnalysis;

namespace QuickTrade.Utilities;

public static class Assert
{
	public static void ArgumentNotNull([NotNull] object? obj)
		=> ArgumentNotNull(obj, null);

	public static void ArgumentNotNull([NotNull] object? obj, string? paramName)
		=> ArgumentNullException.ThrowIfNull(obj, paramName);

	public static void ArgumentInRange([DoesNotReturnIf(false)] bool condition)
		=> ArgumentInRange(condition, null);

	public static void ArgumentInRange([DoesNotReturnIf(false)] bool condition, string? paramName)
	{
		if (condition)
			return;

		throw new ArgumentOutOfRangeException(paramName);
	}

	public static void Argument([DoesNotReturnIf(false)] bool condition)
		=> Argument(condition, null);

	public static void Argument([DoesNotReturnIf(false)] bool condition, string? paramName)
	{
		if (condition)
			return;

		throw new ArgumentException(null, paramName);
	}

	public static void That([DoesNotReturnIf(false)] bool condition)
		=> That(condition, null);

	public static void That([DoesNotReturnIf(false)] bool condition, string? message)
	{
		if (condition)
			return;

		Fail(message ?? "Condition not met.");
	}

	public static void NotNull([NotNull] object? obj)
		=> NotNull(obj, null);

	public static void NotNull([NotNull] object? obj, string? message)
	{
		if (obj is not null)
			return;

		Fail(message ?? "Non-null value expected.");
	}

	[DoesNotReturn]
	public static void Fail(string? message)
		=> throw new AssertionFailedException(message);

	[DoesNotReturn]
	public static void Fail()
	   => Fail(null);
}
