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
            var randY = Random.Range(0, _info.MapSize - startingRoom.Length);
            DrawRoom(randX, randY, startingRoom);
        }

        /// <summary>
        /// Draw a room on the map
        /// </summary>
        /// <remarks>Doesn't check if the position is valid/empty</remarks>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="roomInfo">Room data</param>
        private void DrawRoom(int x, int y, string[] roomInfo)
        {
            for (var yPos = y; yPos < y + roomInfo.Length; yPos++)
            {
                for (var xPos = x; xPos < x + roomInfo[yPos - y].Length; xPos++)
                {
                    _map[yPos][xPos] = new(_info.ParsingData.FirstOrDefault(pd => pd.Character == roomInfo[yPos - y][xPos - x]).Type);
                }
            }
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
                        Gizmos.color = _info.ParsingData.FirstOrDefault(pd => pd.Type == _map[y][x].Type).GizmoColor;
                        Gizmos.DrawCube(new Vector2(x, y), Vector2.one);
                    }
                }
            }
        }
    }
}
