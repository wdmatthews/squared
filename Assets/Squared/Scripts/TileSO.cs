using UnityEngine;

namespace Squared
{
    [CreateAssetMenu(fileName = "New Tile", menuName = "Squared/Tile")]
    public class TileSO : ScriptableObject
    {
        public int BaseNumber = 2;
        public bool CanMerge = true;
        public bool CanSlide = true;
        public Tile Prefab = null;
    }
}
