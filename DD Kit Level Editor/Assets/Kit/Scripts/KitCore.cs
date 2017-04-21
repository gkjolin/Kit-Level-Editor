using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Kit
{
    public class KitCore : MonoBehaviour
    {
        public Kit kit;
        public string levelName = "NewLevel", levelAuthor = "User", levelSource = "127.0.0.0", kitName = "DD Default";
        public bool levelIsValid;
        public List<KitPiece> pieces = new List<KitPiece>();
        public KitRequiredPiece[] requiredPieces;
        int needsToLoad, requiredToLoad;
        public delegate void BakeAction();
        public static event BakeAction OnBaked;

        public bool IsLevelValid
        {
            get
            {
                foreach (var requiredPiece in requiredPieces)
                {
                    if (requiredPiece.Amount != requiredPiece.maxAmount)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void LoadedMaterial()
        {
            needsToLoad--;
            if (needsToLoad == 0)
            {
                Bake();
            }
        }

        public void Bake()
        {
            if (OnBaked != null)
            {
                OnBaked();
            }
        }

        public KitPiece AddPiece(string pieceName, Vector3 position, Quaternion rotation, string materialName, string preset)
        {
            KitPiece instance = Instantiate(Resources.Load("Kit Pieces/" + kitName + "/" + pieceName, typeof(KitPiece)), position, rotation) as KitPiece;
            instance.PieceName = pieceName;
            if (instance.isMaterialCustomizable)
            {
                Object[] procedureMaterialObjects = Resources.LoadAll("Kit Materials/" + kitName + "/" + materialName, typeof(ProceduralMaterial));
                instance.Material = (ProceduralMaterial)procedureMaterialObjects[0];
                instance.Material.name = materialName;
                instance.preset = preset;
            }
            pieces.Add(instance);
            return instance;
        }

        public void RemovePiece(KitPiece piece)
        {
            Destroy(piece.gameObject);
            pieces.Remove(piece);
        }

        public void RemoveAllPieces()
        {
            foreach (KitPiece piece in pieces)
            {
                Destroy(piece.gameObject);
            }
            pieces.Clear();
        }

        public void ReadLevelFile()
        {
            RemoveAllPieces();
            string rt = File.ReadAllText(@"" + Application.dataPath + "/" + levelName + ".rrdddlevel");
            var readText = rt.Split('~');
            {
                var ss = readText[0].Split(';');
                levelName = ss[0];
                levelAuthor = ss[1];
                levelSource = ss[2];
                kitName = ss[3];
                levelIsValid = bool.Parse(ss[4]);
            }
            requiredToLoad = readText.Length - 1;
            needsToLoad = requiredToLoad;
            for (int i = 1; i < readText.Length; i++)
            {
                var ss = readText[i].Split(';');
                var piece = AddPiece(ss[0], StringToVector3(ss[1]), Quaternion.Euler(StringToVector3(ss[2])), ss[4], ss[5]);
            }
        }

        public void GenerateLevelFile()
        {
            var l = (levelName + ";" + levelAuthor + ";" + levelSource + ";" + kitName + ";" + IsLevelValid.ToString() + ";~");
            foreach (KitPiece piece in pieces)
            {
                var materialOptions = "0;";
                if (piece.isMaterialCustomizable)
                {
                    materialOptions = "1;" + piece.Material.name + ";";
                }
                var line = piece.PieceName + ";" + piece.PiecePosition.ToString() + ";" + piece.PieceEulerAngles.ToString() + ";" + materialOptions + piece.preset + ";~";
                l += line;
            }
            l = l.Substring(0, l.Length - 1);
            File.WriteAllText(@"" + Application.dataPath + "/" + levelName + ".rrdddlevel", l);
        }

        public static Vector3 StringToVector3(string sVector)
        {
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }
            string[] sArray = sVector.Split(',');
            Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));
            return result;
        }
    }

    [System.Serializable]
    public struct KitRequiredPiece
    {
        public int minAmount, maxAmount;
        public KitPiece piece;
        int amount;

        public int Amount { get; set; }
    }
}
