using UnityEngine;
using TMPro;

namespace Squared
{
    [AddComponentMenu("Squared/Tile")]
    [DisallowMultipleComponent]
    public class Tile : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] protected SpriteRenderer _renderer = null;
        [SerializeField] protected TextMeshPro _label = null;
        [SerializeField] protected Sprite[] _powerSprites = { };
        #endregion

        #region Runtime Fields
        protected TileSO _data = null;
        protected int _power = 1;
        #endregion

        #region Properties
        protected TileSO Data
        {
            get => _data;
            set
            {
                _data = value;
                _power = 1;
                _renderer.color = _data.Color;
                _label.text = $"{_data.BaseNumber}";
            }
        }
        #endregion

        #region Protected Methods
        protected void IncreasePower()
        {
            _renderer.sprite = _powerSprites[_power];
            _power++;
            _label.text = $"{Pow(_data.BaseNumber, _power)}";
        }

        protected int Pow(int baseNumber, int power)
        {
            int result = baseNumber;

            for (int i = 1; i < power; i++)
            {
                result *= baseNumber;
            }

            return result;
        }
        #endregion
    }
}
