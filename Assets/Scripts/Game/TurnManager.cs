using System.Collections.Generic;
using System.Linq;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Map;
using UnityEngine;

namespace TouhouPrideGameJam4.Game
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        public ACharacter Player { set; private get; }
        private List<ACharacter> _enemies = new();

        public void AddEnemy(ACharacter character)
        {
            _enemies.Add(character);
        }

        private void Awake()
        {
            Instance = this;
        }

        public void MovePlayer(int relX, int relY)
        {
            var newX = Player.Position.x + relX;
            var newY = Player.Position.y + relY;

            if (_enemies.Any(e => e.Position.x == newX && e.Position.y == newY)) // Enemy on the way, we attack it
            {
                // TODO
            }
            else if (MapManager.Instance.IsTileWalkable(newX, newY)) // Nothing here, we can move
            {
                Player.Position = new(newX, newY);
            }
        }
    }
}
