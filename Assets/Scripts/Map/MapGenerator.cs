using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.SO;
using UnityEngine;
using UnityEngine.Assertions;

namespace TouhouPrideGameJam4.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private MapInfo _info;

        [SerializeField]
        private GameObject _player;

        private Tile[][] _map;

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
            var doors = DrawRoom(randX, 0, startingRoom);
            foreach (var d in doors)
            {
                _map[d.Y][d.X].Type = TileType.Breakpoint;
            }

            // Spawn player
            List<Utils.Vector2<int>> possibleSpawnPoints = new();
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
            Instantiate(_player, new Vector3(currentSpawn.X, currentSpawn.Y), Quaternion.identity);
        }

        /// <summary>
        /// Draw a room on the map
        /// </summary>
        /// <remarks>Doesn't check if the position is valid/empty</remarks>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="roomInfo">Room data</param>
        private Utils.Vector2<int>[] DrawRoom(int x, int y, string[] roomInfo)
        {
            // Drawing the room
            for (var yPos = y; yPos < y + roomInfo.Length; yPos++)
            {
                for (var xPos = x; xPos < x + roomInfo[yPos - y].Length; xPos++)
                {
                    _map[yPos][xPos] = new(_info.ParsingData.FirstOrDefault(pd => pd.Character == roomInfo[yPos - y][xPos - x]).Type);
                }
            }

            // Look for all exits
            List<Utils.Vector2<int>> exits = new();
            for (var yPos = y; yPos < y + roomInfo.Length; yPos++)
            {
                for (var xPos = x; xPos < x + roomInfo[yPos - y].Length; xPos++)
                {
                    if (roomInfo[yPos - y][xPos - x] != ' ') // No need to check elements outside of the current room
                    {
                        // For a door to be around us, we need to be on a floor tile
                        if (_map[yPos][xPos].Type == TileType.Empty)
                        {
                            var upType = yPos > 0 ? _map[yPos - 1][xPos]?.Type ?? null : null;
                            var downType = yPos < _info.MapSize - 2 ? _map[yPos + 1][xPos]?.Type ?? null : null;
                            var leftType = xPos > 0 ? _map[yPos][xPos - 1]?.Type ?? null : null;
                            var rightType = xPos < _info.MapSize - 2 ? _map[yPos][xPos + 1]?.Type ?? null : null;

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
                            Gizmos.color = _info.ParsingData.FirstOrDefault(pd => pd.Type == _map[y][x].Type).GizmoColor;
                            Gizmos.DrawCube(new Vector2(x, y), Vector2.one);
                        }
                    }
                }
            }
        }
    }
}
