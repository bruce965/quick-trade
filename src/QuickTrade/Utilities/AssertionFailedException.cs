// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/util/raw/master/LICENSE

namespace QuickTrade.Utilities;

[Serializable]
public class AssertionFailedException : Exception
{
	public AssertionFailedException() : this("Assertion failed.") { }

	public AssertionFailedException(string? message) : base(message) { }

	public AssertionFailedException(string? message, Exception? inner) : base(message, inner) { }

	protected AssertionFailedException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
