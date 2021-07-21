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
        [SerializeField] private float _slideCooldown = 1;
        #endregion

        #region Runtime Fields
        private Vector3 _worldZeroPosition = new Vector3(-1.5f, -1.5f);
        private Dictionary<int, TileSO> _tileSOsByBaseNumber = new Dictionary<int, TileSO>();
        private List<Tile> _tiles = new List<Tile>();
        private Dictionary<Vector2Int, Tile> _tilesByPosition = new Dictionary<Vector2Int, Tile>();
        private Dictionary<TileSO, Stack<Tile>> _inactiveTiles = new Dictionary<TileSO, Stack<Tile>>();
        private float _slideCooldownTimer = 0;
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

        private void Update()
        {
            _slideCooldownTimer = Mathf.Clamp(_slideCooldownTimer - Time.deltaTime, 0, _slideCooldown);
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
            PlaceTile(_tileSOs[1], new Vector2Int(2, 2));
            PlaceTile(_tileSOs[1], new Vector2Int(3, 3));
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
            Tile tileToMergeWith = null;
            tile.NextBoardPosition = position;
            Vector2Int nextPosition = position + direction;

            while (nextPosition.x >= 0 && nextPosition.x < _boardSize.x
                && nextPosition.y >= 0 && nextPosition.y < _boardSize.y)
            {
                tile.NextBoardPosition = nextPosition;

                if (_tilesByPosition.ContainsKey(nextPosition))
                {
                    Tile otherTile = _tilesByPosition[nextPosition];

                    if (tile.Data.BaseNumber == otherTile.Data.BaseNumber && tile.NextPower == otherTile.NextPower)
                    {
                        tileToMergeWith = otherTile;
                    }
                    else tile.NextBoardPosition = nextPosition - direction;

                    break;
                }
                else nextPosition += direction;
            }

            _tilesByPosition.Remove(tile.BoardPosition);

            if (tileToMergeWith)
            {
                tile.RemoveAfterMove = true;
                tileToMergeWith.NextPower++;
                Vector3 mergePosition = tileToMergeWith.transform.position;
                mergePosition.z = -tileToMergeWith.NextPower;
                tileToMergeWith.transform.position = mergePosition;
                _inactiveTiles[tile.Data].Push(tile);
                _tilesByPosition.Remove(tile.BoardPosition);
                _tiles.Remove(tile);
            }
            else
            {
                _tilesByPosition.Add(tile.NextBoardPosition, tile);
            }

            tile.Move(BoardToWorldPosition(tile.NextBoardPosition), tile.NextBoardPosition);
        }
        #endregion

        #region Public Methods
        public void SlideTiles(Vector2Int direction)
        {
            if (!Mathf.Approximately(_slideCooldownTimer, 0)) return;
            _slideCooldownTimer = _slideCooldown;

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

            foreach (var tile in _tiles)
            {
                if (tile.NextPower != tile.Power)
                {
                    tile.SetPower(tile.NextPower);
                }
            }
        }
        #endregion
    }
}
