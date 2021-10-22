using System;
using System.IO;
using System.Text.RegularExpressions;

namespace cmdwtf.UnityTools
{
	public static class StringExtensions
	{
		public static string RemoveNonHexCharacters(this string s, bool allowSpaces)
			=> allowSpaces
				? Regex.Replace(s, "[^a-fA-F0-9 ]+", "", RegexOptions.Compiled)
				: Regex.Replace(s, "[^a-fA-F0-9]+", "", RegexOptions.Compiled);

		public static string InsertSpaces(this string s)
			=> s.InsertSpacesEvery(2);

		public static string InsertSpacesEvery(this string s, int charactersBetweenSpaces)
			=> Regex.Replace(s, @$"(.{{{charactersBetweenSpaces}}})", "$1 ");

		public static byte[] HexStringToByteArray(this string hex)
		{
			const int hexadecimalBase = 16;
			hex = hex.RemoveNonHexCharacters(allowSpaces: false);

			// make sure we have an even length string.
			if (hex.Length == 1)
			{
				hex = "0" + hex;
			}

			if (hex.Length % 2 == 1)
			{
				hex += "0";
			}

			int byteCount = hex.Length/2;
			byte[] result = new byte[byteCount];
			using var reader = new StringReader(hex);

			for (int i = 0; i < byteCount; i++)
			{
				char upperNibble = (char)reader.Read();
				char lowerNibble = (char)reader.Read();
				string byteString = $"{upperNibble}{lowerNibble}";
				result[i] = Convert.ToByte(byteString, hexadecimalBase);
			}

			return result;
		}
	}
}
