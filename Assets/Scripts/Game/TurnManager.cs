using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.SO;
using UnityEngine;

namespace TouhouPrideGameJam4.Game
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [SerializeField]
        private AIInfo _aiInfo;

        [SerializeField]
        private TMP_Text _debugText;

        [SerializeField]
        private InventoryUI _inventory;

        private ACharacter _player;
        public ACharacter Player
        {
            set
            {
                _player = value;
                _player.ShowItems(_inventory);
                UpdateDebugText();
            }
            get => _player;
        }
        private readonly List<ACharacter> _enemies = new();

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
            PlayerEnemyTurn();
        }

        private void PlayerEnemyTurn()
        {
            Vector2Int[] directions = new[]
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            foreach (var enemy in _enemies)
            {
                if (Vector2.Distance(enemy.Position, Player.Position) < _aiInfo.MaxDistanceToMove)
                {
                    var dirTarget = directions.OrderBy(d => Vector2.Distance(enemy.Position + d, Player.Position));
                    foreach (var d in dirTarget)
                    {
                        if (_enemies.FirstOrDefault(e => e.Position.x == enemy.Position.x + d.x && e.Position.y == enemy.Position.y + d.y))
                        {
                            // An enemy is obstructing the way
                            continue;
                        }
                        if (Player.Position.x == enemy.Position.x + d.x && Player.Position.y == enemy.Position.y + d.y)
                        {
                            enemy.Attack(Player);
                            UpdateDebugText();
                            break;
                        }
                        else if (MapManager.Instance.IsTileWalkable(enemy.Position.x + d.x, enemy.Position.y + d.y))
                        {
                            enemy.Position = new(enemy.Position.x + d.x, enemy.Position.y + d.y);
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateDebugText()
        {
            _debugText.text = Player.ToString();
        }
    }
}
