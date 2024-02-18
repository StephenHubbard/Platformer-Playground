using UnityEditor;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;

public static class Setup 
{
   [MenuItem("Tools/Setup/Create Default Folder Structure", priority = 0)]
   public static void CreateDefaultFolder()
   {
      Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings", "Scenes");
      Refresh();
   }

   static class Folders
   {
      public static void CreateDefault(string root, params string[] folders)
      {
         var fullpath = Combine(Application.dataPath, root);
         foreach (var folder in folders)
         {
            var path = Combine(fullpath, folder);
            if (!Exists(path))
            {
               CreateDirectory(path);
            }
         }
      }
   }
}

