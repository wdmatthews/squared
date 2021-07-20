using UnityEngine;

namespace Squared
{
    [CreateAssetMenu(fileName = "New Tile", menuName = "Squared/Tile")]
    public class TileSO : ScriptableObject
    {
        public int BaseNumber = 2;
        public Color Color = new Color(33 / 255f, 150 / 255f, 243 / 255f);
        public Tile Prefab = null;
    }
}
