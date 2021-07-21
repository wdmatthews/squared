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
            //for (int i = 0; i < 4; i++)
            //{
            //    Vector2Int position = new Vector2Int(Random.Range(0, _boardSize.x), Random.Range(0, _boardSize.y));

            //    while (_tilesByPosition.ContainsKey(position))
            //    {
            //        position = new Vector2Int(Random.Range(0, _boardSize.x), Random.Range(0, _boardSize.y));
            //    }

            //    PlaceTile(_tileSOs[0], position);
            //}
            PlaceTile(_tileSOs[0], new Vector2Int(0, 0));
            PlaceTile(_tileSOs[0], new Vector2Int(1, 1));
            PlaceTile(_tileSOs[0], new Vector2Int(2, 2));
            PlaceTile(_tileSOs[0], new Vector2Int(3, 3));
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

        private void TrySlideTile(Vector2Int position, Vector2Int direction)
        {
            Tile tile = _tilesByPosition[position];
            tile.NextBoardPosition = position;
            Vector2Int nextPosition = position + direction;
            bool shouldMerge = false;

            while (nextPosition.x >= 0 && nextPosition.x < _boardSize.x
                && nextPosition.y >= 0 && nextPosition.y < _boardSize.y)
            {
                tile.NextBoardPosition = nextPosition;
                if (_tilesByPosition.ContainsKey(nextPosition))
                {
                    Debug.Log("Attempt merge or stop");
                    shouldMerge = true;
                    break;
                }
                else nextPosition += direction;
            }

            if (!shouldMerge)
            {
                _tilesByPosition.Remove(tile.BoardPosition);
                tile.Move(BoardToWorldPosition(tile.NextBoardPosition), tile.NextBoardPosition);
                _tilesByPosition.Add(tile.BoardPosition, tile);
            }
        }
        #endregion

        #region Public Methods
        public void SlideTiles(Vector2Int direction)
        {
            if (direction.x == -1)
            {
                for (int x = 1; x < _boardSize.x; x++)
                {
                    for (int y = 0; y < _boardSize.y; y++)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        if (!_tilesByPosition.ContainsKey(position)) continue;
                        TrySlideTile(position, direction);
                    }
                }
            }
            else if (direction.x == 1)
            {
                for (int x = _boardSize.x - 2; x >= 0; x--)
                {
                    for (int y = 0; y < _boardSize.y; y++)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        if (!_tilesByPosition.ContainsKey(position)) continue;
                        TrySlideTile(position, direction);
                    }
                }
            }
            else if (direction.y == -1)
            {
                for (int y = 1; y < _boardSize.y; y++)
                {
                    for (int x = 0; x < _boardSize.x; x++)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        if (!_tilesByPosition.ContainsKey(position)) continue;
                        TrySlideTile(position, direction);
                    }
                }
            }
            else if (direction.y == 1)
            {
                for (int y = _boardSize.y - 2; y >= 0; y--)
                {
                    for (int x = 0; x < _boardSize.x; x++)
                    {
                        Vector2Int position = new Vector2Int(x, y);
                        if (!_tilesByPosition.ContainsKey(position)) continue;
                        TrySlideTile(position, direction);
                    }
                }
            }

            //List<Tile> movableTiles = new List<Tile>(_tiles);
            //int slideIteration = 0;

            //for (int i = movableTiles.Count - 1; i >= -1; i--)
            //{
            //    if (movableTiles.Count == 1) i = 0;
            //    Tile tile = movableTiles[i];
            //    if (slideIteration == 0) tile.NextBoardPosition = tile.BoardPosition + direction;

            //    if (tile.NextBoardPosition.x >= 0 && tile.NextBoardPosition.x < _boardSize.x
            //        && tile.NextBoardPosition.y >= 0 && tile.NextBoardPosition.y < _boardSize.y)
            //    {
            //        if (_tilesByPosition.ContainsKey(tile.NextBoardPosition))
            //        {
            //            Tile otherTile = _tilesByPosition[tile.NextBoardPosition];
            //            movableTiles.RemoveAt(i);

            //            if (tile.Data.BaseNumber == otherTile.Data.BaseNumber && tile.Power == otherTile.Power)
            //            {
            //                otherTile.NextPower++;
            //                tile.Remove();
            //                _inactiveTiles[tile.Data].Push(tile);
            //                _tilesByPosition.Remove(tile.BoardPosition);
            //                _tiles.Remove(tile);
            //            }
            //        }
            //        else
            //        {
            //            _tilesByPosition.Remove(tile.BoardPosition);
            //            tile.BoardPosition = tile.NextBoardPosition;
            //            _tilesByPosition.Add(tile.BoardPosition, tile);
            //            tile.NextBoardPosition += direction;
            //        }
            //    }
            //    else
            //    {
            //        movableTiles.RemoveAt(i);
            //    }

            //    if (i == 0)
            //    {
            //        slideIteration++;
            //        int movableCount = movableTiles.Count;
            //        if (movableCount > 0) i = movableCount - 1;
            //        else break;
            //    }
            //}

            //for (int i = _tiles.Count - 1; i >= 0; i--)
            //{
            //    Tile tile = _tiles[i];

            //    if (tile.BoardPosition != tile.NextBoardPosition)
            //    {
            //        tile.Move(BoardToWorldPosition(tile.NextBoardPosition), tile.NextBoardPosition);
            //    }

            //    if (tile.Power != tile.NextPower)
            //    {
            //        if (tile.SetPower(tile.NextPower))
            //        {
            //            _inactiveTiles[tile.Data].Push(tile);
            //            _tilesByPosition.Remove(tile.BoardPosition);
            //            _tiles.RemoveAt(i);
            //        }
            //    }
            //}
        }
        #endregion
    }
}
