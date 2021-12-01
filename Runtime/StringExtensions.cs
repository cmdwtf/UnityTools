using System;
using System.IO;
using System.Text.RegularExpressions;

namespace cmdwtf.UnityTools
{
	public static class StringExtensions
	{
		private static readonly Regex CamelCase = new Regex(@"(\B[A-Z])", RegexOptions.Compiled);
		private static readonly Regex HexWithSpace = new Regex("[^a-fA-F0-9 ]+", RegexOptions.Compiled);
		private static readonly Regex HexWithoutSpace = new Regex("[^a-fA-F0-9 ]+", RegexOptions.Compiled);

		private const string SpaceAfterReplacement = @"$1 ";
		private const string SpaceBeforeReplacement = @" $1";
		private const string RemoveReplacement = @"";

		public static string RemoveNonHexCharacters(this string s, bool allowSpaces)
			=> allowSpaces
				? HexWithSpace.Replace(s, RemoveReplacement)
				: HexWithoutSpace.Replace(s, RemoveReplacement);

		public static string InsertSpaces(this string s)
			=> s.InsertSpacesEvery(2);

		public static string InsertSpacesEvery(this string s, int charactersBetweenSpaces)
			=> Regex.Replace(s, @$"(.{{{charactersBetweenSpaces}}})", SpaceAfterReplacement);

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

		public static UnityEngine.GUIContent ToGUIContent(this string text)
			=> new UnityEngine.GUIContent(text);

		public static string SpaceCamelCase(this string camelCaseString)
			=> CamelCase.Replace(camelCaseString, SpaceBeforeReplacement);
	}
}
