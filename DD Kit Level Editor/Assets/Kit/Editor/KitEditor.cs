using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Kit
{
    [CustomEditor(typeof(Kit))]
    public class KitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Kit kit = (Kit)target;
            if (GUILayout.Button("Refresh"))
            {
                var path = Application.dataPath + "/Kit/Resources/Kit Pieces/" + kit.name + "/";
                List<string> items = new List<string>();
                items.AddRange(Directory.GetFiles(path));
                var i = 0;
                while (i < items.Count)
                {
                    if (items[i].Contains(".meta"))
                    {
                        items.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
                i = 0;
                while (i < items.Count)
                {
                    items[i] = Path.GetFileNameWithoutExtension(items[i]);
                    i++;
                }

                kit.pieces = items.ToArray();

                path = Application.dataPath + "/Kit/Resources/Kit Materials/" + kit.name + "/";
                items.Clear();
                items.AddRange(Directory.GetDirectories(path));
                i = 0;
                while (i < items.Count)
                {
                    items[i] = items[i].Replace(path, "");
                    i++;
                }
                EditorUtility.SetDirty(kit);
                kit.materials = items.ToArray();
            }
        }
    }
}
