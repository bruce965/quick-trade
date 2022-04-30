// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/util/raw/master/LICENSE

using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace QuickTrade.Core;

/// <summary>
/// Thin wrapper around a <see cref="long"/> counting the number of
/// seconds elapsed since the midnight of January 1st, 1970 UTC.
/// </summary>
[Serializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public struct UnixTimestamp : IEquatable<UnixTimestamp>, IComparable<UnixTimestamp>, IConvertible, ISpanFormattable, ISerializable
{
	readonly long _epochSeconds;

	public UnixTimestamp(long epochSeconds)
	{
		_epochSeconds = epochSeconds;
	}

	public override string ToString()
		=> ((DateTime)this).ToString("yyyy-MM-ddTHH-mm-ssZ", CultureInfo.InvariantCulture);

	#region IEquatable

	public bool Equals(UnixTimestamp other)
		=> _epochSeconds == other._epochSeconds;

	public override bool Equals(object? obj)
		=> obj is UnixTimestamp other && Equals(other);

	public override int GetHashCode()
		=> _epochSeconds.GetHashCode();

	public static bool operator ==(UnixTimestamp left, UnixTimestamp right)
		=> left.Equals(right);

	public static bool operator !=(UnixTimestamp left, UnixTimestamp right)
		=> !left.Equals(right);

	#endregion

	#region IComparable

	public int CompareTo(UnixTimestamp other)
		=> _epochSeconds.CompareTo(other._epochSeconds);

	public static bool operator <(UnixTimestamp left, UnixTimestamp right)
		=> left._epochSeconds < right._epochSeconds;

	public static bool operator <=(UnixTimestamp left, UnixTimestamp right)
		=> left._epochSeconds <= right._epochSeconds;

	public static bool operator >(UnixTimestamp left, UnixTimestamp right)
		=> left._epochSeconds > right._epochSeconds;

	public static bool operator >=(UnixTimestamp left, UnixTimestamp right)
		=> left._epochSeconds >= right._epochSeconds;

	#endregion

	#region IConvertible

	TypeCode IConvertible.GetTypeCode() => _epochSeconds.GetTypeCode();

	bool IConvertible.ToBoolean(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToBoolean(provider);

	byte IConvertible.ToByte(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToByte(provider);

	char IConvertible.ToChar(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToChar(provider);

	DateTime IConvertible.ToDateTime(IFormatProvider? provider)
		=> this;

	decimal IConvertible.ToDecimal(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToDecimal(provider);

	double IConvertible.ToDouble(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToDouble(provider);

	short IConvertible.ToInt16(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToInt16(provider);

	int IConvertible.ToInt32(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToInt32(provider);

	long IConvertible.ToInt64(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToInt64(provider);

	sbyte IConvertible.ToSByte(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToSByte(provider);

	float IConvertible.ToSingle(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToSingle(provider);

	string IConvertible.ToString(IFormatProvider? provider)
		=> ((DateTime)this).ToString(provider);

	object IConvertible.ToType(Type conversionType, IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToType(conversionType, provider);

	ushort IConvertible.ToUInt16(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToUInt16(provider);

	uint IConvertible.ToUInt32(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToUInt32(provider);

	ulong IConvertible.ToUInt64(IFormatProvider? provider)
		=> ((IConvertible)_epochSeconds).ToUInt64(provider);

	#endregion

	#region IFormattable

	public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		=> ((DateTime)this).TryFormat(destination, out charsWritten, format, provider);

	public string ToString(string? format, IFormatProvider? formatProvider)
		=> ((DateTime)this).ToString(format, formatProvider);

	#endregion

	#region ISerializable

	UnixTimestamp(SerializationInfo info, StreamingContext context)
	{
		_epochSeconds = info.GetInt64("ms");
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("ms", _epochSeconds);
	}

	#endregion

	#region Debug

	string GetDebuggerDisplay()
		=> ToString();

	#endregion

	#region Casts

	public static implicit operator DateTime(UnixTimestamp timestamp)
		=> DateTime.UnixEpoch + TimeSpan.FromTicks(timestamp._epochSeconds * TimeSpan.TicksPerSecond);

	public static implicit operator UnixTimestamp(DateTime date)
		=> new((DateTime.UnixEpoch - date).Ticks / TimeSpan.TicksPerSecond);

	public static implicit operator long(UnixTimestamp timestamp)
		=> timestamp._epochSeconds;

	public static implicit operator UnixTimestamp(long epochSeconds)
		=> new(epochSeconds);

	public static explicit operator int(UnixTimestamp timestamp)
		=> (int)timestamp._epochSeconds;

	public static implicit operator UnixTimestamp(int epochSeconds)
		=> new(epochSeconds);

	#endregion

	#region Operators

	public static TimeSpan operator -(UnixTimestamp left, UnixTimestamp right)
		=> TimeSpan.FromTicks((left._epochSeconds - right._epochSeconds) * TimeSpan.TicksPerSecond);

	public static UnixTimestamp operator +(UnixTimestamp timestamp, TimeSpan time)
		=> timestamp + (time.Ticks / TimeSpan.TicksPerSecond);

	public static UnixTimestamp operator +(UnixTimestamp timestamp, long seconds)
		=> timestamp._epochSeconds + seconds;

	public static UnixTimestamp operator -(UnixTimestamp timestamp, TimeSpan time)
		=> timestamp + (-time);

	public static UnixTimestamp operator -(UnixTimestamp timestamp, long seconds)
		=> timestamp + (-seconds);

	#endregion
}
