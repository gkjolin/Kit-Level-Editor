using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    public class KitPieceRequire : MonoBehaviour
    {
        KitCore core;
        KitPiece piece;

        private void Start()
        {
            core = FindObjectOfType(typeof(KitCore)) as KitCore;
            piece = GetComponent<KitPiece>();
            for (int i = 0; i < core.requiredPieces.Length; i++)
            {
                if (core.requiredPieces[i].piece.PieceName == piece.PieceName)
                {
                    core.requiredPieces[i].Amount++;
                    if (core.requiredPieces[i].Amount > core.requiredPieces[i].maxAmount)
                    {
                        core.RemovePiece(piece);
                    }
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < core.requiredPieces.Length; i++)
            {
                if (core.requiredPieces[i].piece.PieceName == piece.PieceName)
                {
                    if (core.requiredPieces[i].Amount > 0)
                    {
                        core.requiredPieces[i].Amount--;
                    }
                    break;
                }
            }
        }
    }
}
