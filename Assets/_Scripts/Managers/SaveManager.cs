using System.IO;
using Data;
using Scripts.Utility;
using Sirenix.Serialization;
using UnityEngine;

namespace Managers
{
	public class SaveManager : PersistentSingleton<SaveManager>
	{
		private const string extension = ".json";
		private const string user_data_path = "/UserData";
		private UserData userData;

		private void OnEnable()
		{
			InitializeData();
		}

		private void InitializeData()
		{
			if (File.Exists(Application.persistentDataPath + user_data_path))
			{
				userData = LoadFromJson<UserData>(user_data_path);
			}
			else
			{
				userData = new UserData();
				SaveToJson(userData, user_data_path);
			}
		}

		public UserData GetUserData()
		{
			if(userData==null)
				InitializeData();
			return userData;
		}

		private void SaveToJson<T>(T data, string path)
		{
			byte[] serializedData = SerializationUtility.SerializeValue(data, DataFormat.JSON);
			File.WriteAllBytes(Application.persistentDataPath + path + extension, serializedData);
		}

		private T LoadFromJson<T>(string path)
		{
			byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + path);
			return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.JSON);
		}
	}


}
