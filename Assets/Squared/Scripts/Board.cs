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
        #endregion

        #region Public Methods
        public void SlideTiles(Vector2Int direction)
        {
            List<Tile> movableTiles = new List<Tile>(_tiles);
            int slideIteration = 0;

            for (int i = movableTiles.Count - 1; i >= -1; i--)
            {
                if (movableTiles.Count == 1) i = 0;
                Tile tile = movableTiles[i];
                if (slideIteration == 0) tile.NextBoardPosition = tile.BoardPosition + direction;

                if (tile.NextBoardPosition.x >= 0 && tile.NextBoardPosition.x < _boardSize.x
                    && tile.NextBoardPosition.y >= 0 && tile.NextBoardPosition.y < _boardSize.y)
                {
                    if (_tilesByPosition.ContainsKey(tile.NextBoardPosition))
                    {
                        Tile otherTile = _tilesByPosition[tile.NextBoardPosition];
                        movableTiles.RemoveAt(i);

                        if (tile.Data.BaseNumber == otherTile.Data.BaseNumber && tile.Power == otherTile.Power)
                        {
                            otherTile.NextPower++;
                            tile.Remove();
                            _inactiveTiles[tile.Data].Push(tile);
                            _tilesByPosition.Remove(tile.BoardPosition);
                            _tiles.Remove(tile);
                        }
                    }
                    else
                    {
                        _tilesByPosition.Remove(tile.BoardPosition);
                        tile.BoardPosition = tile.NextBoardPosition;
                        _tilesByPosition.Add(tile.BoardPosition, tile);
                        tile.NextBoardPosition += direction;
                    }
                }
                else
                {
                    movableTiles.RemoveAt(i);
                }

                if (i == 0)
                {
                    slideIteration++;
                    int movableCount = movableTiles.Count;
                    if (movableCount > 0) i = movableCount - 1;
                    else break;
                }
            }

            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                Tile tile = _tiles[i];

                if (tile.BoardPosition != tile.NextBoardPosition)
                {
                    tile.Move(BoardToWorldPosition(tile.BoardPosition), tile.BoardPosition);
                }

                if (tile.Power != tile.NextPower)
                {
                    if (tile.SetPower(tile.NextPower))
                    {
                        _inactiveTiles[tile.Data].Push(tile);
                        _tilesByPosition.Remove(tile.BoardPosition);
                        _tiles.RemoveAt(i);
                    }
                }
            }
        }
        #endregion
    }
}
