using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.SO;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.UIElements.NavigationMoveEvent;

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
        private List<Room> _rooms = new();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
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

            // Place the next rooms
            for (int c = 10; c > 0; c--)
            {
                var r = _rooms[^1];
                foreach (var d in GetFreeDoors(r, true))
                {
                    if (d.Direction == Direction.Down)
                    {
                        var possibilities = GetRandomMatchingRoom(d);
                        var randRoom = possibilities[Random.Range(0, possibilities.Length)];
                        DrawRoom(randRoom);
                        _rooms.Add(randRoom);
                        break;
                    }
                }
            }

            // Replace empty spaces by walls so the player can't exit the map
            foreach (var r in _rooms)
            {
                foreach (var d in GetFreeDoors(r, true))
                {
                    _map[d.Y][d.X].Type = TileType.Wall;
                }
            }

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
            TurnManager.Instance.Player = Instantiate(_player, new Vector3(currentSpawn.x, currentSpawn.y), Quaternion.identity).GetComponent<PlayerController>();
            TurnManager.Instance.Player.Position = new(currentSpawn.x, currentSpawn.y);
        }

        /// <summary>
        /// Get a random room that fit in the map
        /// </summary>
        /// <param name="door">Door we are starting at</param>
        private Room[] GetRandomMatchingRoom(Door door)
        {
            return _info.Rooms.SelectMany(r =>
            {
                var data = GetRoom(r);
                return GetPlacementOffset(door, new(0, 0, data)).Select(o => new Room(o.x, o.y, data)).ToArray();
            }).ToArray();
        }

        /// <summary>
        /// Get the offset needed to place our room
        /// </summary>
        /// <param name="door">Door we need to match</param>
        /// <param name="targetRoom">Information about our room</param>
        private Vector2Int[] GetPlacementOffset(Door door, Room targetRoom)
        {
            var doors = GetFreeDoors(targetRoom, false);
            var oppositeDoors = door.Direction switch
            {
                Direction.Up => doors.Where(x => x.Direction == Direction.Down),
                Direction.Down => doors.Where(x => x.Direction == Direction.Up),
                Direction.Left => doors.Where(x => x.Direction == Direction.Right),
                Direction.Right => doors.Where(x => x.Direction == Direction.Left),
                _ => throw new System.NotImplementedException()
            };
            List<Vector2Int> positions = new();
            foreach (var d in oppositeDoors)
            {
                int xOffset = door.X;
                int yOffset = door.Y;
                positions.Add(new(xOffset - d.X, yOffset + d.Y));
            }
            return positions.ToArray();
        }

        public bool IsTileWalkable(int x, int y)
            => x >= 0 && x < _info.MapSize && y >= 0 && y < _info.MapSize &&
               _map[y][x] != null &&
               LookupTileByType(_map[y][x].Type).CanBeWalkedOn;

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
        private Door[] GetFreeDoors(Room room, bool validatePosOnMap)
        {
            List<Door> exits = new();
            for (var yPos = room.Y; yPos < room.Y + room.Data.Length; yPos++)
            {
                var relativeY = yPos - room.Y;
                for (var xPos = room.X; xPos < room.X + room.Data[relativeY].Length; xPos++)
                {
                    var relativeX = xPos - room.X;

                    if (room.Data[relativeY][relativeX] != ' ') // No need to check elements outside of the current room
                    {
                        // For a door to be around us, we need to be on a floor tile
                        if ((validatePosOnMap && _map[yPos][xPos].Type == TileType.Empty) ||
                            (!validatePosOnMap && LookupTileByChar(room.Data[relativeY][relativeX]).Type == TileType.Empty))
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
                                downType = relativeY < room.Data.Length - 1 ? LookupTileByChar(room.Data[relativeY + 1][relativeX])?.Type ?? null : null;
                                leftType = relativeX > 0 ? LookupTileByChar(room.Data[relativeY][relativeX - 1])?.Type ?? null : null;
                                rightType = relativeX < room.Data[relativeY].Length - 1 ? LookupTileByChar(room.Data[relativeY][relativeX + 1])?.Type ?? null : null;
                            }

                            // A door need to be surrounded by 2 walls for the frame and then an empty tile and an unallocated one
                            // Example:
                            // XXX
                            //  D.
                            //  XX
                            // Here D is a valid door

                            if (upType == TileType.Wall && downType == TileType.Wall)
                            {
                                if (leftType == TileType.Empty && rightType == null)
                                {
                                    exits.Add(new(xPos, yPos, Direction.Right));
                                }
                                else if (leftType == null && rightType == TileType.Empty)
                                {
                                    exits.Add(new(xPos, yPos, Direction.Left));
                                }
                            }
                            else if (leftType == TileType.Wall && rightType == TileType.Wall)
                            {
                                if (upType == TileType.Empty && downType == null)
                                {
                                    exits.Add(new(xPos, yPos, Direction.Down));
                                }
                                else if (upType == null && downType == TileType.Empty)
                                {
                                    exits.Add(new(xPos, yPos, Direction.Up));
                                }
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

        private TileData LookupTileByChar(char c)
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
