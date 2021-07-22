using System.Collections.Generic;
using UnityEngine;

namespace Squared
{
    [AddComponentMenu("Squared/Board")]
    [DisallowMultipleComponent]
    public class Board : MonoBehaviour
    {
        #region Inspector Fields
        public static int LevelSOIndex = 0;
        [SerializeField] private SpriteRenderer _renderer = null;
        [SerializeField] private Vector2Int _boardSize = new Vector2Int(4, 4);
        [SerializeField] private TileSO[] _tileSOs = { };
        [SerializeField] private float _slideCooldown = 1;
        [SerializeField] private float _retryCooldown = 1;
        [SerializeField] private LevelSO[] _levelSOs = { };
        #endregion

        #region Runtime Fields
        private Vector3 _worldZeroPosition = new Vector3(-1.5f, -1.5f);
        private List<Tile> _tiles = new List<Tile>();
        private List<Tile> _standardTiles = new List<Tile>();
        private Dictionary<Vector2Int, Tile> _tilesByPosition = new Dictionary<Vector2Int, Tile>();
        private Dictionary<TileSO, Stack<Tile>> _inactiveTiles = new Dictionary<TileSO, Stack<Tile>>();
        private float _slideCooldownTimer = 0;
        private float _retryCooldownTimer = 0;
        private bool _isRetrying = false;
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
            StartLevel();
        }

        private void Update()
        {
            _slideCooldownTimer = Mathf.Clamp(_slideCooldownTimer - Time.deltaTime, 0, _slideCooldown);

            if (_isRetrying)
            {
                _retryCooldownTimer = Mathf.Clamp(_retryCooldownTimer - Time.deltaTime, 0, _retryCooldown);

                if (Mathf.Approximately(_retryCooldownTimer, 0))
                {
                    _isRetrying = false;
                    StartLevel();
                }
            }
        }
        #endregion

        #region Private Methods
        private void ReadTileSOs()
        {
            foreach (var tileSO in _tileSOs)
            {
                _inactiveTiles.Add(tileSO, new Stack<Tile>());
            }
        }

        private void PlaceInitialTiles(string tilemap)
        {
            string[] rows = tilemap.Split('\n');
            int y = _boardSize.y - 1;

            foreach (var row in rows)
            {
                for (int x = row.Length - 1; x >= 0; x--)
                {
                    string cell = $"{row[x]}";
                    if (cell == " ") continue;
                    PlaceTile(_tileSOs[int.Parse(cell)], new Vector2Int(x, y));
                }

                y--;
            }
        }

        private void StartLevel()
        {
            PlaceInitialTiles(_levelSOs[LevelSOIndex].Tilemap);
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
            _tilesByPosition.Add(position, tile);
            if (tile.Data.CanMerge) _standardTiles.Add(tile);
        }

        private void RemoveTile(Tile tile)
        {
            _inactiveTiles[tile.Data].Push(tile);
            _tilesByPosition.Remove(tile.BoardPosition);
            _tiles.Remove(tile);
            if (tile.Data.CanMerge) _standardTiles.Remove(tile);
        }

        private void TrySlideTile(Vector2Int position, Vector2Int direction)
        {
            Tile tile = _tilesByPosition[position];
            if (!tile.Data.CanSlide) return;
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

                    if (tile.Data.BaseNumber == otherTile.Data.BaseNumber && tile.NextPower == otherTile.NextPower
                        && tile.Data.CanMerge && otherTile.Data.CanMerge)
                    {
                        tileToMergeWith = otherTile;
                    }
                    else tile.NextBoardPosition = nextPosition - direction;

                    break;
                }
                else nextPosition += direction;
            }

            _tilesByPosition.Remove(tile.BoardPosition);

            if (tileToMergeWith) MergeTiles(tile, tileToMergeWith);
            else _tilesByPosition.Add(tile.NextBoardPosition, tile);

            tile.Move(BoardToWorldPosition(tile.NextBoardPosition), tile.NextBoardPosition);
        }

        private void MergeTiles(Tile tile1, Tile tile2)
        {
            tile1.RemoveAfterMove = true;
            tile2.NextPower++;
            Vector3 mergePosition = tile2.transform.position;
            mergePosition.z = -tile2.NextPower;
            tile2.transform.position = mergePosition;
            RemoveTile(tile1);
        }

        private void CheckIfGameEnded()
        {
            if (_standardTiles.Count == 0)
            {
                // TODO: Show win screen
                Debug.Log("Level completed!");
            }
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

            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                Tile tile = _tiles[i];

                if (tile.NextPower != tile.Power)
                {
                    if (tile.SetPower(tile.NextPower))
                    {
                        RemoveTile(tile);
                        CheckIfGameEnded();
                    }
                }
            }
        }

        public void Retry()
        {
            if (_isRetrying) return;

            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                Tile tile = _tiles[i];
                tile.Remove();
                RemoveTile(tile);
            }

            _isRetrying = true;
            _retryCooldownTimer = _retryCooldown;
        }
        #endregion
    }
}
