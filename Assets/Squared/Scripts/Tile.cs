using UnityEngine;
using TMPro;

namespace Squared
{
    [AddComponentMenu("Squared/Tile")]
    [DisallowMultipleComponent]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer = null;
        [SerializeField] private TextMeshPro _label = null;
    }
}
