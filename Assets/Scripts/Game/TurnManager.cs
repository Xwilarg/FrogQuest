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

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Add a new enemy to the list of enemies
        /// </summary>
        public void AddEnemy(ACharacter character)
        {
            _enemies.Add(character);
        }

        public void RemoveCharacter(ACharacter character)
        {
            if (character.GetInstanceID() == Player.GetInstanceID()) // The player died, gameover
            {
                throw new System.NotImplementedException("Player died");
            }
            else
            {
                _enemies.RemoveAll(x => x.GetInstanceID() == character.GetInstanceID());
            }
            Destroy(character.gameObject);
        }

        /// <summary>
        /// Move the player in the world
        /// </summary>
        /// <param name="relX">Relative X position</param>
        /// <param name="relY">Relative Y position</param>
        public void MovePlayer(int relX, int relY)
        {
            var newX = Player.Position.x + relX;
            var newY = Player.Position.y + relY;

            var target = _enemies.FirstOrDefault(e => e.Position.x == newX && e.Position.y == newY);
            if (target != null) // Enemy on the way, we attack it
            {
                Player.Attack(target);
            }
            else if (MapManager.Instance.IsTileWalkable(newX, newY)) // Nothing here, we can move
            {
                Player.Position = new(newX, newY);
            }
        }
    }
}
