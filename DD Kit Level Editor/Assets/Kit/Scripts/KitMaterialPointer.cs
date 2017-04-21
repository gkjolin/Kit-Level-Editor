using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    [ExecuteInEditMode]
    public class KitMaterialPointer : MonoBehaviour
    {
        public KitMaterialWindow matEditor;

        private void OnEnable()
        {
            matEditor = transform.root.GetComponentInChildren<KitMaterialWindow>();
        }
    }
}
