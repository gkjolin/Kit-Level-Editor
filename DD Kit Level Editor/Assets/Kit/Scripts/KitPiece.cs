using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    public class KitPiece : MonoBehaviour
    {
        public string PieceName
        {
            get
            {
                return gameObject.name;
            }
            set
            {
                gameObject.name = value;
            }
        }

        public Vector3 PiecePosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Vector3 PieceEulerAngles
        {
            get
            {
                return transform.eulerAngles;
            }
            set
            {
                transform.eulerAngles = value;
            }
        }

        public ProceduralMaterial Material
        {
            get
            {
                return rend.material as ProceduralMaterial;
            }
            set
            {
                rend.material = value;
            }
        }

        public Renderer rend;

        public string preset = "";
        public bool isMaterialCustomizable = true;
        public float tiling = 1;
    }
}
