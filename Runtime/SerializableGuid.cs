using System;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Serializable wrapper for System.Guid.
	/// Can be implicitly converted to/from System.Guid.
	///
	/// Based on https://forum.unity.com/threads/cannot-serialize-a-guid-field-in-class.156862/#post-6996680 by Searous
	/// </summary>
	[Serializable]
	public struct SerializableGuid : ISerializationCallbackReceiver
	{
		private Guid _guid;

		[SerializeField]
		internal string serializedGuid;

		public SerializableGuid(Guid guid)
		{
			_guid = guid;
			serializedGuid = null;
		}

		public override bool Equals(object other)
			=> other is SerializableGuid sguid &&
			   _guid.Equals(sguid._guid);

		public override int GetHashCode()
			=> -1324198676 + _guid.GetHashCode();

		public void OnAfterDeserialize()
		{
			try
			{
				_guid = new Guid(serializedGuid);
			}
			catch
			{
				_guid = Guid.Empty;
				Debug.LogWarning(
					$"Attempted to parse invalid GUID {serializedGuid.GetType().Name} '{serializedGuid}'. GUID will set to System.Guid.Empty");
			}
		}

		public void OnBeforeSerialize() => serializedGuid = SerializeGuid(_guid);

		private static string SerializeGuid(Guid toSerialize) => toSerialize.ToString();

		public override string ToString() => _guid.ToString();

		public static bool operator ==(SerializableGuid a, SerializableGuid b) => a._guid == b._guid;
		public static bool operator !=(SerializableGuid a, SerializableGuid b) => a._guid != b._guid;
		public static bool operator ==(SerializableGuid a, Guid b) => a._guid == b;
		public static bool operator !=(SerializableGuid a, Guid b) => a._guid != b;
		public static bool operator ==(Guid a, SerializableGuid b) => a == b._guid;
		public static bool operator !=(Guid a, SerializableGuid b) => a != b._guid;

		public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);
		public static implicit operator Guid(SerializableGuid serializable) => serializable._guid;

		public static implicit operator SerializableGuid(string serializedGuid)
			=> new SerializableGuid(Guid.Parse(serializedGuid));

		public static implicit operator string(SerializableGuid serializedGuid)
			=> serializedGuid.ToString();

		public static explicit operator SerializableGuid(byte[] bytes)
			=> new SerializableGuid(new Guid(bytes));

		public static implicit operator byte[](SerializableGuid serializedGuid)
			=> serializedGuid._guid.ToByteArray();
	}
}
