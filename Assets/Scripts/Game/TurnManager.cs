﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.SO;
using UnityEngine;
using static UnityEngine.UIElements.NavigationMoveEvent;

namespace TouhouPrideGameJam4.Game
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [SerializeField]
        private AIInfo _aiInfo;

        [SerializeField]
        private InventoryUI _inventory;

        [SerializeField]
        private TMP_Text _objectiveText;

        [SerializeField]
        private GameObject _damageIndicator;

        private string _baseObjectiveText;

        private ACharacter _player;
        public ACharacter Player
        {
            set
            {
                _player = value;
                _characters.Add(value);
            }
            get => _player;
        }
        private readonly List<ACharacter> _characters = new();

        private void Awake()
        {
            Instance = this;
            _baseObjectiveText = _objectiveText.text;
        }

        public void ToggleInventory()
        {
            _inventory.gameObject.SetActive(!_inventory.gameObject.activeInHierarchy);
            if (_inventory.gameObject.activeInHierarchy)
            {
                Player.ShowItems(_inventory, null);
            }
        }

        public void SpawnDamageText(int amount, Color color, float x, float y)
        {
            var go = Instantiate(_damageIndicator, new(x, y), Quaternion.identity);
            var text = go.GetComponent<TMP_Text>();
            text.color = color;
            text.text = amount.ToString();
        }

        /// <summary>
        /// Add a new enemy to the list of enemies
        /// </summary>
        public void AddCharacter(ACharacter character)
        {
            _characters.Add(character);
            _objectiveText.text = _baseObjectiveText.Replace("{0}", _characters.Where(x => x.Team == Team.Enemies).Count().ToString());
        }

        public void RemoveCharacter(ACharacter character)
        {
            if (character.GetInstanceID() == Player.GetInstanceID()) // The player died, gameover
            {
                throw new System.NotImplementedException("Player died");
            }
            else
            {
                _characters.RemoveAll(x => x.GetInstanceID() == character.GetInstanceID());
                _objectiveText.text = _baseObjectiveText.Replace("{0}", _characters.Where(x => x.Team == Team.Enemies).Count().ToString());
            }
            Destroy(character.gameObject);
        }

        public void TryAttackCharacter(int x, int y, int damage)
        {
            if (Player.Position.x == x && Player.Position.y == y)
            {
                Player.TakeDamage(null, damage);
            }
            else
            {
                var target = _characters.FirstOrDefault(e => e.Position.x == x && e.Position.y == y);
                if (target != null)
                {
                    target.TakeDamage(null, damage);
                }
            }
        }

        public void SetDirection(ACharacter character, int x, int y)
        {
            if (x < 0) character.Direction = Direction.Left;
            else if (x > 0) character.Direction = Direction.Right;
            else if (y < 0) character.Direction = Direction.Down;
            else if (y > 0) character.Direction = Direction.Up;
            else throw new System.NotImplementedException();
        }

        /// <summary>
        /// Move the player in the world
        /// </summary>
        /// <param name="relX">Relative X position</param>
        /// <param name="relY">Relative Y position</param>
        public bool MovePlayer(int relX, int relY)
        {
            var didMove = false;
            if (Player.CanDoSomething())
            {
                var newX = Player.Position.x + relX;
                var newY = Player.Position.y + relY;

                var target = _characters.FirstOrDefault(e => e.Position.x == newX && e.Position.y == newY);
                var content = MapManager.Instance.GetContent(newX, newY);
                if (target != null) // Enemy on the way, we attack it
                {
                    Player.Attack(target);
                }
                else if (content == TileContentType.Door)
                {
                    MapManager.Instance.OpenDoor(newX, newY);
                }
                else if (Player.CanMove() && MapManager.Instance.IsTileWalkable(newX, newY)) // Nothing here, we can move
                {
                    Player.Position = new(newX, newY);
                    didMove = true;
                }
                SetDirection(Player, relX, relY);
            }
            Player.EndTurn();
            PlayEnemyTurn();

            return didMove;
        }

        public void PlayEnemyTurn()
        {
            Vector2Int[] directions = new[]
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            for (int i = _characters.Count - 1; i >= 0; i--)
            {
                var c = _characters[i];
                // We ignore player or if the current character is disabled
                if (c.GetInstanceID() == Player.GetInstanceID() || !c.gameObject.activeInHierarchy || !c.CanDoSomething())
                {
                    continue;
                }

                // Our target is the closest character with a different team than ours
                var target = _characters.Where(x => x.Team != c.Team && x.gameObject.activeInHierarchy).OrderBy(x => Vector2.Distance(c.Position, x.Position)).FirstOrDefault();

                if (target == null)
                {
                    continue;
                }

                // If the enemy is close enough
                if (Vector2.Distance(c.Position, target.Position) < _aiInfo.MaxDistanceToMove)
                {
                    bool didPlay = false;
                    var dirTarget = directions.OrderBy(d => Vector2.Distance(c.Position + d, target.Position));

                    // We try to attack him
                    foreach (var d in dirTarget)
                    {
                        if (target.Position.x == c.Position.x + d.x && target.Position.y == c.Position.y + d.y)
                        {
                            c.Attack(target);
                            SetDirection(c, d.x, d.y);
                            if (c.Info.DoesDisappearAfterAttacking)
                            {
                                RemoveCharacter(c);
                            }
                            didPlay = true;
                            break;
                        }
                    }

                    // Else we try to move towards its position
                    if (!didPlay && c.CanMove())
                    {
                        foreach (var d in dirTarget)
                        {
                            if (_characters.FirstOrDefault(e => e.Position.x == c.Position.x + d.x && e.Position.y == c.Position.y + d.y))
                            {
                                // An enemy is obstructing the way
                                continue;
                            }
                            else if (MapManager.Instance.IsTileWalkable(c.Position.x + d.x, c.Position.y + d.y))
                            {
                                c.Position = new(c.Position.x + d.x, c.Position.y + d.y);
                                SetDirection(c, d.x, d.y);
                                break;
                            }
                        }
                    }
                }
                c.EndTurn();
            }
        }

        public ACharacter GetCharactertPos(int x, int y)
            => _characters.FirstOrDefault(e => e.Position.x == x && e.Position.y == y);
    }
}
