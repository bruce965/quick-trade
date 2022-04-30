// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/util/raw/master/LICENSE

using System.Text;

namespace QuickTrade.Utilities;

static class StringExtensions
{
	/// <summary>
	/// Escape a string to be used as the content of an XML node.
	/// 
	/// <code>
	/// "banana".EscapeXml();  // "banana"
	/// "&lt;test&gt;".EscapeXml();  // "&amp;lt;test&amp;gt;"
	/// </code>
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static string ToXmlEscaped(this string str)
	{
		var el = new System.Xml.XmlDocument().CreateElement("x");
		el.InnerText = str;
		return el.OuterXml["<x>".Length..^"<x></x>".Length];
	}

	/// <summary>
	/// Convert a string to a C# string literal.
	/// 
	/// <code>
	/// "banana".ToCSharpStringLiteral();  // "\"banana\""
	/// "aaa\t\r\nbbb".ToCSharpStringLiteral();  // "\"aaa\\t\\r\\nbbb\""
	/// @"test""123".ToCSharpStringLiteral();  // "\"test\\\"123\""
	/// </code>
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	public static string ToCSharpStringLiteral(this string? str)
	{
		if (str == null)
			return "null";

		var builder = new StringBuilder(str.Length + 10);

		builder.Append('"');

		foreach (var chr in str)
		{
			var needsEscape = char.IsControl(chr) || chr == '\\' || chr == '\"';

			if (needsEscape)
			{
				var escaped = chr switch
				{
					'"' => "\\\"",
					'\\' => "\\\\",
					'\0' => "\\0",
					'\a' => "\\a",
					'\b' => "\\b",
					'\f' => "\\f",
					'\n' => "\\n",
					'\r' => "\\r",
					'\t' => "\\t",
					'\v' => "\\v",
					var c when c <= 0xFFFF => $"\\u{(int)c:X4}",
					_ => $"\\U{(int)chr:X8}"
				};

				builder.Append(escaped);
			}
			else
			{
				builder.Append(chr);
			}

		}

		builder.Append('"');

		return builder.ToString();
	}
}
