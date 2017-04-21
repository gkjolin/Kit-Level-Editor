using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    public class KitBaker : MonoBehaviour
    {
        void OnEnable()
        {
            KitCore.OnBaked += Bake;
        }

        void OnDisable()
        {
            KitCore.OnBaked -= Bake;
        }

        void Bake()
        {
            var r = GetComponent<ReflectionProbe>();
            r.RenderProbe();
        }
    }
}
