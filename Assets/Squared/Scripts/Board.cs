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
        private List<Tile> _tiles = new List<Tile>();
        private Dictionary<Vector2Int, Tile> _tilesByPosition = new Dictionary<Vector2Int, Tile>();
        private Dictionary<TileSO, Stack<Tile>> _inactiveTiles = new Dictionary<TileSO, Stack<Tile>>();
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
        private void Start()
        {
            ReadTileSOs();
            PlaceInitialTiles();
        }
        #endregion

        #region Private Methods
        private void ReadTileSOs()
        {
            foreach (var tileSO in _tileSOs)
            {
                _tileSOsByBaseNumber.Add(tileSO.BaseNumber, tileSO);
                _inactiveTiles.Add(tileSO, new Stack<Tile>());
            }
        }

        private void PlaceInitialTiles()
        {
            // TEMPORARY LOGIC; REPLACE LATER
            for (int i = 0; i < 4; i++)
            {
                Vector2Int position = new Vector2Int(Random.Range(0, _boardSize.x), Random.Range(0, _boardSize.y));

                while (_tilesByPosition.ContainsKey(position))
                {
                    position = new Vector2Int(Random.Range(0, _boardSize.x), Random.Range(0, _boardSize.y));
                }

                PlaceTile(_tileSOs[0], position);
            }
        }

        private Vector3 BoardToWorldPosition(Vector2Int boardPosition)
        {
            return new Vector3(
                boardPosition.x + _worldZeroPosition.x,
                boardPosition.y + _worldZeroPosition.y
            );
        }

        private Vector2Int WorldToBoardPosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.x - _worldZeroPosition.x),
                Mathf.RoundToInt(worldPosition.y - _worldZeroPosition.y)
            );
        }

        private void PlaceTile(TileSO tileSO, Vector2Int position)
        {
            Tile tile = null;

            if (_inactiveTiles[tileSO].Count > 0)
            {
                tile = _inactiveTiles[tileSO].Pop();
            }
            else
            {
                tile = Instantiate(tileSO.Prefab, transform);
            }

            tile.Place(BoardToWorldPosition(position), tileSO, position);
            _tiles.Add(tile);
            _tilesByPosition[position] = tile;
        }

        private void MergeTiles(List<Tile> tiles)
        {

        }
        #endregion

        #region Public Methods
        public void SlideTiles(Vector2Int direction)
        {
            foreach (var tile in _tiles)
            {
                Vector2Int position = tile.BoardPosition;
                Vector2Int nextPosition = position + direction;

                while (nextPosition.x >= 0 && nextPosition.x < _boardSize.x
                    && nextPosition.y >= 0 && nextPosition.y < _boardSize.y)
                {
                    position = nextPosition;
                    nextPosition += direction;
                }

                if (_tilesByPosition.ContainsKey(position))
                {
                    Debug.Log("Merge");
                }
                else
                {
                    _tilesByPosition.Remove(tile.BoardPosition);
                    tile.Move(BoardToWorldPosition(position), position);
                    _tilesByPosition.Add(position, tile);
                }
            }
        }
        #endregion
    }
}
