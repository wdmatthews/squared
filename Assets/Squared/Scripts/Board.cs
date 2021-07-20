using System.Collections.Generic;
using UnityEngine;

namespace Squared
{
    [AddComponentMenu("Squared/Board")]
    [DisallowMultipleComponent]
    public class Board : MonoBehaviour
    {
        #region Inspector Fields
        [SerializeField] private SpriteRenderer _renderer = null;
        [SerializeField] private Vector2Int _boardSize = new Vector2Int(4, 4);
        [SerializeField] private TileSO[] _tileSOs = { };
        #endregion

        #region Runtime Fields
        private Vector3 _worldZeroPosition = new Vector3(-1.5f, -1.5f);
        private Dictionary<int, TileSO> _tileSOsByBaseNumber = new Dictionary<int, TileSO>();
        #endregion

        #region Properties
        public Vector2Int BoardSize
        {
            get => _boardSize;
            set
            {
                _boardSize = value;
                _renderer.size = _boardSize;
                _worldZeroPosition = new Vector3(-_boardSize.x / 2 + 0.5f, -_boardSize.y / 2 + 0.5f);
            }
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            ReadTileSOs();
        }
        #endregion

        #region Private Methods
        private void ReadTileSOs()
        {
            foreach (var tileSO in _tileSOs)
            {
                _tileSOsByBaseNumber.Add(tileSO.BaseNumber, tileSO);
            }
        }
        #endregion

        #region Public Methods
        public Vector3 BoardToWorldPosition(Vector2Int boardPosition)
        {
            return new Vector3(
                boardPosition.x + _worldZeroPosition.x,
                boardPosition.y + _worldZeroPosition.y
            );
        }

        public Vector2Int WorldToBoardPosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x - _worldZeroPosition.x),
                Mathf.RoundToInt(worldPosition.y - _worldZeroPosition.y)
            );
        }
        #endregion
    }
}
