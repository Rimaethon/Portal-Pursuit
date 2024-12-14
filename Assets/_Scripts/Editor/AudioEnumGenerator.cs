using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Editor
{
    [CustomEditor(typeof(AudioLibrary))]
    public class AudioEnumGenerator : UnityEditor.Editor
    {
        const string directory_path = "Assets/_Scripts/Enums";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AudioLibrary sfxLibrary = (AudioLibrary)target;

            if (GUILayout.Button("Generate Enums"))
                GenerateEnums(sfxLibrary);
        }

        private void GenerateEnums(AudioLibrary sfxLibrary)
        {
            foreach (FieldInfo fieldInfo in sfxLibrary.GetType().GetFields())
            {
                if (fieldInfo.FieldType != typeof(AudioClip[])) continue;
                string arrayName = fieldInfo.Name;
                string enumCode = $"public enum {arrayName}\n{{\n";

                AudioClip[] audioClips = (AudioClip[])fieldInfo.GetValue(sfxLibrary);

                for (int i = 0; i < audioClips.Length; i++) enumCode += $"\t{audioClips[i].name} = {i},\n";

                enumCode += "}";

                Debug.Log($"Generated Enum for {arrayName}:\n{enumCode}");

                SaveEnumToFile(arrayName, enumCode);
            }
        }

        private void SaveEnumToFile(string arrayName, string enumCode)
        {
            string filePath = $"{directory_path}/{arrayName}.cs";

            if (!AssetDatabase.IsValidFolder(directory_path))
                AssetDatabase.CreateFolder("Assets/_Scripts/", "Enums");

            File.WriteAllText(filePath, enumCode);
            AssetDatabase.Refresh();
            Debug.Log($"Saved Enum for {arrayName} to {filePath}");
        }
    }
}
