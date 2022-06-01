using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.SO;
using UnityEngine;
using UnityEngine.Assertions;

namespace TouhouPrideGameJam4.Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        [SerializeField]
        private MapInfo _info;

        [SerializeField]
        private GameObject _player;

        private Tile[][] _map;

        private Vector2Int _playerPos;
        private List<Room> _rooms = new();

        private void Awake()
        {
            Instance = this;

            Assert.IsNotNull(_info, "MapInfo is not set");

            // Init map
            _map = new Tile[_info.MapSize][];
            for (int i = 0; i < _map.Length; i++)
            {
                _map[i] = new Tile[_info.MapSize];
            }

            // Spawn starting room
            var startingRoom = GetRoom(_info.StartingRoom);
            var randX = Random.Range(0, _info.MapSize - startingRoom[0].Length);
            var roomObj = new Room(randX, 0, startingRoom);
            DrawRoom(roomObj);
            _rooms.Add(roomObj);

            // Spawn player
            List<Vector2Int> possibleSpawnPoints = new();
            for (int y = 0; y < _map.Length; y++)
            {
                for (int x = 0; x < _map[y].Length; x++)
                {
                    if (_map[y][x] != null && _map[y][x].Type == TileType.StartingPos)
                    {
                        possibleSpawnPoints.Add(new(x, y));
                    }
                }
            }
            Assert.IsTrue(possibleSpawnPoints.Any(), "No spawn point found");
            var currentSpawn = possibleSpawnPoints[Random.Range(0, possibleSpawnPoints.Count)];
            _playerPos = new(currentSpawn.x, currentSpawn.y);
            Instantiate(_player, new Vector3(currentSpawn.x, currentSpawn.y), Quaternion.identity);

            // DEBUG: Replace free doors by breakpoints
            foreach (var r in _rooms)
            {
                foreach (var d in GetFreeDoors(r, true))
                {
                    _map[d.y][d.x].Type = TileType.Breakpoint;
                }
            }
        }

        public Vector2Int MovePlayer(int x, int y)
        {
            var newX = _playerPos.x + x;
            var newY = _playerPos.y + y;

            // Tile is in bound and can be walked on
            if (newX >= 0 && newX < _info.MapSize && newY >= 0 && newY < _info.MapSize &&
                _map[newY][newX] != null &&
                LookupTileByType(_map[newY][newX].Type).CanBeWalkedOn)
            {
                _playerPos = new(newX, newY);
            }

            return _playerPos;
        }

        /// <summary>
        /// Draw a room on the map
        /// </summary>
        /// <remarks>Doesn't check if the position is valid/empty</remarks>
        /// <param name="room">Room to check</param>
        private void DrawRoom(Room room)
        {
            for (var yPos = room.Y; yPos < room.Y + room.Data.Length; yPos++)
            {
                for (var xPos = room.X; xPos < room.X + room.Data[yPos - room.Y].Length; xPos++)
                {
                    _map[yPos][xPos] = new(LookupTileByChar(room.Data[yPos - room.Y][xPos - room.X]).Type);
                }
            }
        }

        /// <summary>
        /// Get all the doors that lead nowhere
        /// </summary>
        /// <param name="room">Room to check</param>
        /// <param name="validatePosOnMap">Do we take the map in consideration or only the current object?</param>
        /// <returns>All positions of free doors</returns>
        private Vector2Int[] GetFreeDoors(Room room, bool validatePosOnMap)
        {
            List<Vector2Int> exits = new();
            for (var yPos = room.Y; yPos < room.Y + room.Data.Length; yPos++)
            {
                var relativeY = yPos - room.Y;
                for (var xPos = room.X; xPos < room.X + room.Data[relativeY].Length; xPos++)
                {
                    var relativeX = xPos - room.X;

                    if (room.Data[relativeY][relativeX] != ' ') // No need to check elements outside of the current room
                    {
                        // For a door to be around us, we need to be on a floor tile
                        if (_map[yPos][xPos].Type == TileType.Empty)
                        {
                            TileType? upType, downType, leftType, rightType;

                            if (validatePosOnMap) // We look what are the adjacent tiles using the map
                            {
                                upType = yPos > 0 ? _map[yPos - 1][xPos]?.Type ?? null : null;
                                downType = yPos < _info.MapSize - 1 ? _map[yPos + 1][xPos]?.Type ?? null : null;
                                leftType = xPos > 0 ? _map[yPos][xPos - 1]?.Type ?? null : null;
                                rightType = xPos < _info.MapSize - 1 ? _map[yPos][xPos + 1]?.Type ?? null : null;
                            }
                            else // We only look for adjacent tiles on the object itself
                            {
                                upType = relativeY > 0 ? LookupTileByChar(room.Data[relativeY - 1][relativeX])?.Type ?? null : null;
                                downType = relativeY < room.Data.Length - 1 ? LookupTileByChar(room.Data[yPos + 1][xPos])?.Type ?? null : null;
                                leftType = relativeX > 0 ? LookupTileByChar(room.Data[yPos][xPos - 1])?.Type ?? null : null;
                                rightType = relativeX < room.Data[relativeY].Length - 1 ? LookupTileByChar(room.Data[yPos][xPos + 1])?.Type ?? null : null;
                            }

                            // A door need to be surrounded by 2 walls for the frame and then an empty tile and an unallocated one
                            // Example:
                            // XXX
                            //  D.
                            //  XX
                            // Here D is a valid door

                            if (
                                // Looking for vertical doors
                                (upType == TileType.Wall && downType == TileType.Wall &&
                                (leftType == TileType.Empty && rightType == null) ||
                                (leftType == null && rightType == TileType.Empty))
                                ||
                                // Looking for horizontal doors
                                (leftType == TileType.Wall && rightType == TileType.Wall &&
                                (upType == TileType.Empty && downType == null) ||
                                (upType == null && downType == TileType.Empty))
                                )
                            {
                                exits.Add(new(xPos, yPos));
                            }
                        }
                    }
                }
            }

            return exits.ToArray();
        }

        /// <summary>
        /// Get a string array symbolising a room
        /// </summary>
        /// <param name="textFile">Text file to get the room from</param>
        private string[] GetRoom(TextAsset textFile)
        {
            return textFile.text.Replace("\r", "").Split('\n');
        }

        private TileData LookupTileByType(TileType type)
            => _info.ParsingData.FirstOrDefault(pd => pd.Type == type);

        private TileData? LookupTileByChar(char c)
            => c == ' ' ? null :_info.ParsingData.FirstOrDefault(pd => pd.Character == c);

        private void OnDrawGizmos()
        {
            if (_map == null)
            {
                return; // Nothing to do in editor mode
            }

            for (int y = 0; y < _map.Length; y++)
            {
                for (int x = 0; x < _map[y].Length; x++)
                {
                    if (_map[y][x] != null) // Null mean tile is empty
                    {
                        if (_map[y][x].Type == TileType.Breakpoint) // Used for debug
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(new Vector2(x, y), .5f);
                        }
                        else
                        {
                            Gizmos.color = LookupTileByType(_map[y][x].Type).GizmoColor;
                            Gizmos.DrawCube(new Vector2(x, y), Vector2.one);
                        }
                    }
                }
            }
        }
    }
}
