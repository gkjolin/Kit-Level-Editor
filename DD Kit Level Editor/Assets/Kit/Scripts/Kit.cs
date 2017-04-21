using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kit
{
    [CreateAssetMenu(fileName = "NewKit", menuName = "Kit/New Kit", order = 1)]
    public class Kit : ScriptableObject
    {
        public string[] pieces;
        public string[] materials;
    }
}
