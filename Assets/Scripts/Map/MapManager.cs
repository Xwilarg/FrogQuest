using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.SO;
using TouhouPrideGameJam4.SO.Item;
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
        private GameObject _prefabPlayer;

        [SerializeField]
        private GameObject _prefabTile, _prefabDoor, _prefabItemFloor;

        private Tile[][] _map;
        private readonly List<Room> _rooms = new();

        private GameObject _tileContainer;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Assert.IsNotNull(_info, "MapInfo is not set");

            _tileContainer = new("Map");

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
            for (int c = _info.IterationCount; c > 0; c--)
            {
                for (int i = _rooms.Count - 1; i >= 0; i--)
                {
                    var r = _rooms[i];
                    foreach (var d in GetFreeDoors(r, true))
                    {
                        if (d.Direction != Direction.Up)
                        {
                            var possibilities = GetRandomMatchingRoom(d);
                            if (possibilities.Any())
                            {
                                var randRoom = possibilities[Random.Range(0, possibilities.Length)];
                                DrawRoom(randRoom);

                                // Add doors to separate rooms
                                SetTileContent(d.X, d.Y, TileContentType.Door);

                                _rooms.Add(randRoom);
                            }
                        }
                    }
                }
            }

            // Replace empty spaces by walls so the player can't exit the map
            var wall = LookupTileByType(TileType.Wall);
            foreach (var r in _rooms)
            {
                foreach (var d in GetFreeDoors(r, true))
                {
                    SetTile(d.X, d.Y, wall);
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
            var character = Instantiate(_prefabPlayer, new Vector3(currentSpawn.x, currentSpawn.y), Quaternion.identity).GetComponent<ACharacter>();
            character.Position = new(currentSpawn.x, currentSpawn.y);
            TurnManager.Instance.Player = character;

            // Spawn enemies
            var enemyContainer = new GameObject("Enemies");
            foreach (var room in _rooms.Skip(1)) // We check each room, skipping the starting one
            {
                var nbEnemies = Random.Range(0, _info.MaxEnemiesPerRoom + 1);
                List<Vector2Int> spawnPos = new();

                while (spawnPos.Count < nbEnemies)
                {
                    var y = Random.Range(1, room.Data.Length - 1);
                    var x = Random.Range(1, room.Data[y].Length - 1);
                    var pos = new Vector2Int(x, y);
                    var tileOk = LookupTileByChar(room.Data[y][x])?.CanBeWalkedOn == true && TurnManager.Instance.GetCharacterPos(x, y) == null;

                    if (!spawnPos.Contains(pos) && tileOk)
                    {
                        spawnPos.Add(new(room.X + x, room.Y + y));
                    }
                }

                foreach (var pos in spawnPos)
                {
                    var sumDrop = _info.EnemiesSpawn.Sum(x => x.Weight);
                    var targetWeight = Random.Range(0, sumDrop) + 1;
                    var index = 0;
                    do
                    {
                        targetWeight -= _info.EnemiesSpawn[index].Weight;
                        index++;
                    } while (targetWeight > 0);
                    index--;
                    var target = _info.EnemiesSpawn[index];
                    var enemy = Instantiate(target.Item, new Vector3(pos.x, pos.y), Quaternion.identity).GetComponent<ACharacter>();
                    enemy.Team = Team.Enemies;
                    enemy.Position = new(pos.x, pos.y);
                    TurnManager.Instance.AddCharacter(enemy);
                    enemy.transform.parent = enemyContainer.transform;
                    enemy.gameObject.SetActive(false);
                }
            }

            // Show spawn room
            DiscoverRoom(currentSpawn.x, currentSpawn.y);
        }

        private void DiscoverRoom(int x, int y)
        {
            // If object is out of bounds or already active in the hierarchy, we stop here
            if (y < 0 || y >= _map.Length || x < 0 || x >= _map[y].Length ||
                _map[y][x] == null || _map[y][x].SpriteRendererMain.gameObject.activeInHierarchy)
            {
                return;
            }

            _map[y][x].SpriteRendererMain.gameObject.SetActive(true);
            if (_map[y][x].SpriteRendererSub != null)
            {
                _map[y][x].SpriteRendererSub.gameObject.SetActive(true);
            }
            if (_map[y][x].SpriteRendererItem != null)
            {
                _map[y][x].SpriteRendererItem.gameObject.SetActive(true);
            }
            var enemy = TurnManager.Instance.GetCharacterPos(x, y);
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
            }

            if (IsTileWalkable(x, y))
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        DiscoverRoom(x + i, y + j);
                    }
                }
            }
        }

        public bool IsAnythingOnFloor(int x, int y) => _map[y][x].ItemDropped != null;

        public void OpenDoor(int x, int y)
        {
            ClearContent(x, y);
            DiscoverRoom(x - 1, y);
            DiscoverRoom(x + 1, y);
            DiscoverRoom(x, y - 1);
            DiscoverRoom(x, y + 1);
        }

        /// <summary>
        /// Get a random room that fit in the map
        /// </summary>
        /// <param name="door">Door we are starting at</param>
        private Room[] GetRandomMatchingRoom(Door door)
        {
            return _info.Rooms
                .SelectMany(r =>
                {
                    // Load information about the rooms and their offset to be placed properly
                    var data = GetRoom(r);
                    return GetPlacementOffset(door, new(0, 0, data)).Select(o => new Room(o.x, o.y, data)).ToArray();
                })
                .Where(room =>
                {
                    // Check if the room can go inside the map without being out of bounds

                    // Check for out of bounds for Y
                    if (room.Y < 0 || room.Y + room.Data.Length > _map.Length)
                    {
                        return false;
                    }

                    for (var yPos = room.Y; yPos < room.Y + room.Data.Length; yPos++)
                    {
                        var relativeY = yPos - room.Y;

                        // Out of bounds for X
                        if (room.X < 0 || room.X + room.Data[relativeY].Length > _map.Length)
                        {
                            return false;
                        }

                        for (var xPos = room.X; xPos < room.X + room.Data[relativeY].Length; xPos++)
                        {
                            var relativeX = xPos - room.X;
                            if (_map[yPos][xPos] != null && _map[yPos][xPos].Type != TileType.Empty && _map[yPos][xPos].Type != LookupTileByChar(room.Data[relativeY][relativeX]).Type)
                            {
                                // For a room to be valid, we must either build it over an empty land or to all tiles to be on their matching counterpart
                                // This is because we are building doorframe over an existing doorframe
                                return false;
                            }
                        }
                    }
                    return true;
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
                if (d.Direction == Direction.Up || d.Direction == Direction.Down)
                {
                    positions.Add(new(xOffset - d.X, yOffset + d.Y));
                }
                else
                {
                    positions.Add(new(xOffset - d.X, yOffset - d.Y));
                }
            }
            return positions.ToArray();
        }

        public TileContentType GetContent(int x, int y)
            => _map[y][x].Content;

        private void ClearContent(int x, int y)
        {
            _map[y][x].Content = TileContentType.None;
            _map[y][x].SpriteRendererSub.sprite = null;
        }

        public bool IsTileWalkable(int x, int y)
            => x >= 0 && x < _info.MapSize && y >= 0 && y < _info.MapSize &&
               _map[y][x] != null &&
               LookupTileByType(_map[y][x].Type).CanBeWalkedOn &&
               _map[y][x].Content == TileContentType.None;

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
                    var tile = LookupTileByChar(room.Data[yPos - room.Y][xPos - room.X]);
                    SetTile(xPos, yPos, tile);
                }
            }
        }

        private void SetTile(int x, int y, TileData tile)
        {
            if (_map[y][x] == null)
            {
                var t = Instantiate(_prefabTile, new(x, y), Quaternion.identity);
                t.transform.parent = _tileContainer.transform;
                _map[y][x] = new(tile.Type, t.GetComponent<SpriteRenderer>());
                t.SetActive(false);
            }
            else
            {
                _map[y][x].Type = tile.Type;
            }
            _map[y][x].SpriteRendererMain.sprite = tile.Sprite;
        }

        public void SetItemOnFloor(int x, int y, AItemInfo item)
        {
            if (_map[y][x].SpriteRendererItem == null)
            {
                _map[y][x].SpriteRendererItem = Instantiate(_prefabItemFloor, new(x, y), Quaternion.identity).GetComponent<SpriteRenderer>();
            }
            _map[y][x].SpriteRendererItem.sprite = item.Sprite;
            _map[y][x].ItemDropped = item;
        }

        public AItemInfo TakeItemFromFloor(int x, int y)
        {
            var item = _map[y][x].ItemDropped;
            _map[y][x].SpriteRendererItem.sprite = null;
            _map[y][x].ItemDropped = null;
            return item;
        }

        private void SetTileContent(int x, int y, TileContentType content)
        {
            var t = Instantiate(content switch
            {
                TileContentType.Door => _prefabDoor,
                _ => throw new System.NotImplementedException()
            }, new(x, y), Quaternion.identity);
            t.transform.parent = _tileContainer.transform;
            _map[y][x].SpriteRendererSub = t.GetComponent<SpriteRenderer>();
            _map[y][x].Content = content;
            t.SetActive(false);
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
                    }
                }
            }
        }
    }
}
