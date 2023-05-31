using System.Collections.Generic;
using UnityEngine;

namespace ScrtipableObjects
{
    
    [CreateAssetMenu(fileName = "NpcSetting", menuName = "NpcSetting", order = 0)]
    public class NpcSetting : ScriptableObject
    {
        [TextArea] public List<string> prompt;
        [Header("Example")] public List<NpcExample> examples;
    }

    [System.Serializable]
    public class NpcExample
    {
        [TextArea] public string question;
        [TextArea] public string answer;
    }
}