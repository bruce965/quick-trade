// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/util/raw/master/LICENSE

using System.Security;

namespace QuickTrade.Utilities;

static class ConsoleInteractive
{
	public delegate bool Validator<T>(string value, out T parsed, out string? error);

	// first value is the short for, second value is the long form
	static readonly IReadOnlyList<string> _trueValues = new[] { "y", "yes", "1", "t", "true" };
	static readonly IReadOnlyList<string> _falseValues = new[] { "n", "no", "0", "f", "false" };

	static readonly string _defaultQuestion = "Value";
	static readonly char? _defaultPunctuation = ':';

	#region Boolean

	public static bool AskBoolean(string label, bool defaultValue, bool autoConfirm = false)
		=> AskBooleanInternal(label, defaultValue, autoConfirm);

	public static bool AskBoolean(string label, bool? defaultValue = null)
		=> AskBooleanInternal(label, defaultValue, false);

	static bool AskBooleanInternal(string label, bool? defaultValue, bool autoConfirm)
	{
		var trueValue = _trueValues[0];
		var falseValue = _falseValues[0];
		string defaultInput = "";

		if (defaultValue == true)
		{
			trueValue = trueValue.ToUpper();
			defaultInput = _trueValues[1];
		}
		else if (defaultValue == false)
		{
			falseValue = falseValue.ToUpper();
			defaultInput = _falseValues[1];
		}

		var prompt = BuildPrompt(label, $"{trueValue}/{falseValue}");

		return AskValue<bool>(ValidateBoolean, prompt, defaultInput, autoConfirm);
	}

	static bool ValidateBoolean(string value, out bool parsed, out string? error)
	{
		if (_falseValues.Contains(value, StringComparer.InvariantCultureIgnoreCase))
		{
			parsed = false;
			error = null;
			return true;
		}

		if (_trueValues.Contains(value, StringComparer.InvariantCultureIgnoreCase))
		{
			parsed = true;
			error = null;
			return true;
		}

		parsed = default;
		error = "Unrecognized value.";
		return false;
	}

	#endregion

	#region String

	public static string AskString(string label, string defaultValue, bool autoConfirm = false)
		=> AskStringInternal(label, defaultValue, autoConfirm);

	public static string AskString(string label, string? defaultValue = null)
		=> AskStringInternal(label, defaultValue, false);

	static string AskStringInternal(string label, string? defaultValue, bool autoConfirm)
	{
		var prompt = BuildPrompt(label, defaultValue);

		return AskValue<string>(ValidateString, prompt, defaultValue ?? "", autoConfirm);
	}

	static bool ValidateString(string value, out string parsed, out string? error)
	{
		parsed = value;
		error = null;
		return true;
	}

	#endregion

	static T AskValue<T>(Validator<T> validator, string prompt, string? defaultInput, bool autoConfirm)
	{
		do
		{
			Console.Error.Write(prompt);

			(int, int)? cursorPosition = null;

			string? input = null;
			if (!autoConfirm)
			{
				cursorPosition = GetCursorPosition();

				var consoleInput = Console.ReadLine();
				if (consoleInput == null)
					throw new EndOfStreamException();

				input = consoleInput.Trim();
			}

			if (string.IsNullOrWhiteSpace(input))
			{
				if (!string.IsNullOrWhiteSpace(defaultInput))
				{
					if (cursorPosition.HasValue)
						SetCursorPosition(cursorPosition.Value);

					input = defaultInput;
					Console.Error.WriteLine(input);
				}
				else
				{
					Console.Error.WriteLine("A value is required.");
					continue;
				}
			}

			if (!validator(input, out var parsed, out var error))
			{
				Console.Error.WriteLine(error);
				continue;
			}

			return parsed;
		}
		while (!autoConfirm);  // avoid infinite loops

		throw new InvalidOperationException();
	}

	static string BuildPrompt(string? label, string? defaultLabel = null)
	{
		if (string.IsNullOrWhiteSpace(defaultLabel))
			return $"{label} ";

		if (string.IsNullOrWhiteSpace(label))
			label = _defaultQuestion;

		string question;
		char? punctuation;

		var lastChar = label[label.Length - 1];
		if (char.IsPunctuation(lastChar))
		{
			question = label.Substring(0, label.Length - 1);
			punctuation = lastChar;
		}
		else
		{
			question = label;
			punctuation = _defaultPunctuation;
		}

		return $"{question} [{defaultLabel}]{punctuation} ";
	}

	static (int Top, int Left)? GetCursorPosition()
	{
		try
		{
			return (Console.CursorTop, Console.CursorLeft);
		}
		catch (SecurityException) { }
		catch (IOException) { }

		return null;
	}

	static void SetCursorPosition((int Top, int Left) position)
	{
		try
		{
			Console.CursorLeft = position.Left;
			Console.CursorTop = position.Top;
		}
		catch (ArgumentOutOfRangeException) { }
		catch (SecurityException) { }
		catch (IOException) { }
	}
}
