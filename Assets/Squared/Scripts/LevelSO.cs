using UnityEngine;

namespace Squared
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Squared/Level")]
    public class LevelSO : ScriptableObject
    {
        [TextArea] public string Tilemap = "";
    }
}
