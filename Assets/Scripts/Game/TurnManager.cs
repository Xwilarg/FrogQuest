using System.Collections.Generic;
using System.Linq;
using TMPro;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Character.AI;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Dialog;
using TouhouPrideGameJam4.Game.Persistency;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.SO.Character;
using TouhouPrideGameJam4.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.NavigationMoveEvent;

namespace TouhouPrideGameJam4.Game
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        [SerializeField]
        private AIInfo _aiInfo;

        [SerializeField]
        private TMP_Text _objectiveText;

        [SerializeField]
        private GameObject _damageIndicator;

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private AudioClip _openDoor;

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

            Player = Instantiate(_playerPrefab).GetComponent<ACharacter>();
        }

        /*public void ToggleInventory()
        {
            _inventory.gameObject.SetActive(!_inventory.gameObject.activeInHierarchy);
            if (_inventory.gameObject.activeInHierarchy)
            {
                Player.ShowItems(_inventory, null);
            }
        }*/

        public void SpawnDamageText(string amount, Color color, float x, float y)
        {
            var go = Instantiate(_damageIndicator, new(x, y), Quaternion.identity);
            var text = go.GetComponent<TMP_Text>();
            text.color = color;
            text.text = amount;
        }

        private void UpdateObjectiveText()
        {
            if (MapManager.Instance.CurrMap.IsBossRoom)
            {
                _objectiveText.text = "Beat Remillia";
            }
            else
            {
                var enemyCount = _characters.Where(x => x.Team == Team.Enemies).Count();
                if (enemyCount == 0)
                {
                    if (MapManager.Instance.CurrentWorld == 1)
                    {
                        StoryManager.Instance.ProgressIsAvailable(StoryProgress.Forest4Kill);
                    }
                    _objectiveText.text = "Find the exit!";
                }
                else
                {
                    if (enemyCount == _totalEnemyCount / 2 && MapManager.Instance.CurrentWorld == 0)
                    {
                        StoryManager.Instance.ProgressIsAvailable(StoryProgress.YoukaiMountain1Half);
                    }
                    _objectiveText.text = _baseObjectiveText.Replace("{0}", _characters.Where(x => x.Team == Team.Enemies).Count().ToString());
                }
            }
        }
        private int _totalEnemyCount;
        public void CountEnemies()
        {
            _totalEnemyCount = _characters.Where(x => x.Team == Team.Enemies).Count();
        }

        /// <summary>
        /// Add a new enemy to the list of enemies
        /// </summary>
        public void AddCharacter(ACharacter character)
        {
            _characters.Add(character);
            UpdateObjectiveText();
        }

        public void RemoveCharacter(ACharacter character)
        {
            if (character.GetInstanceID() == Player.GetInstanceID()) // The player died, gameover
            {
                PersistencyManager.Instance.TotalEnergy += ((PlayerController)Player).Energy;
                SceneManager.LoadScene("MenuRuns");
            }
            else
            {
                _characters.RemoveAll(x => x.GetInstanceID() == character.GetInstanceID());
                var enemyCount = _characters.Where(x => x.Team == Team.Enemies).Count();
                if (enemyCount == 0)
                {
                    MapManager.Instance.EnableGoal();
                    if (MapManager.Instance.GetContent(Player.Position.x, Player.Position.y) == TileContentType.ExitEnabled)
                    {
                        MapManager.Instance.GoToNextZone();
                    }
                }
                UpdateObjectiveText();
            }
            Destroy(character.gameObject);
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
        public bool MovePlayer(int relX, int relY, bool moveOnly)
        {
            var newX = Player.Position.x + relX;
            var newY = Player.Position.y + relY;
            var didMove = false;
            if (Player.CanDoSomething())
            {
                ACharacter target = null;

                for (int r = 1; r <= Player.EquippedWeapon.Range; r++)
                {
                    target = _characters.FirstOrDefault(e => e.Position.x == Player.Position.x + relX * r && e.Position.y == Player.Position.y + relY * r);
                    if (target != null)
                    {
                        break;
                    }
                }
                var content = MapManager.Instance.GetContent(newX, newY);
                if (target != null) // Enemy on the way, we attack it
                {
                    if (!moveOnly)
                    {
                        Player.Attack(target);
                    }
                }
                else if (content == TileContentType.Door)
                {
                    if (!moveOnly)
                    {
                        MapManager.Instance.OpenDoor(newX, newY);
                        SoundManager.Instance.PlayOpenContainerClip(_openDoor);
                    }
                }
                else if (content == TileContentType.Chest)
                {
                    if (!moveOnly)
                    {
                        MapManager.Instance.OpenChest(newX, newY);
                        SoundManager.Instance.PlayOpenContainerClip(_openDoor);
                    }
                }
                else
                {
                    didMove = TryMove(newX, newY);
                }
                SetDirection(Player, relX, relY);
            }
            Player.EndTurn();
            PlayEnemyTurn();

            return didMove;
        }

        private bool TryMove(int newX, int newY)
        {
            if (Player.CanMove() && MapManager.Instance.IsTileWalkable(newX, newY)) // Nothing here, we can move
            {
                Player.Position = new(newX, newY);
                if (MapManager.Instance.GetContent(newX, newY) == TileContentType.ExitEnabled)
                {
                    MapManager.Instance.GoToNextZone();
                }
                return true;
            }
            return false;
        }

        private void NormalAI(ACharacter c, Vector2Int[] directions, IOrderedEnumerable<ACharacter> targets)
        {
            var enemy = (Enemy)c;

            // If the enemy is close enough
            if (Vector2.Distance(c.Position, targets.First().Position) < _aiInfo.MaxDistanceToMove)
            {
                bool didPlay = false;
                //var dirTarget = directions.OrderBy(d => Vector2.Distance(c.Position + d, target.Position)); // TODO: ???

                // We try to attack him
                foreach (var d in directions)
                {
                    for (int r = 1; r <= c.EquippedWeapon.Range; r++)
                    {
                        var x = c.Position.x + (d.x * r);
                        var y = c.Position.y + (d.y * r);
                        if (MapManager.Instance.IsTileWalkable(x, y))
                        {
                            var target = targets.FirstOrDefault(o => o.Position.x == x && o.Position.y == y);
                            if (target != null && (!c.EquippedWeapon.IsHeal || !target.IsHealthFull)) // We found a target and either it's an enemy, or an allie with health not full
                            {
                                if (enemy.AttackCharge < c.Info.TimeBeforeAttack)
                                {
                                    enemy.AttackCharge++;
                                }
                                else
                                {
                                    c.Attack(target);
                                    SetDirection(c, d.x, d.y);
                                    if (c.Info.DoesDisappearAfterAttacking)
                                    {
                                        RemoveCharacter(c);
                                    }
                                    enemy.AttackCharge = 0;
                                }
                                didPlay = true;
                                break;
                            }
                        }
                    }
                }

                // Else we try to move towards its position
                if (!didPlay)
                {
                    if (enemy.AttackCharge > 0)
                    {
                        enemy.AttackCharge = 0;
                    }
                    else
                    {
                        if (c.CanMove())
                        {
                            foreach (var d in directions.OrderBy(d => Vector2.Distance(c.Position + d, targets.First().Position)))
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
                }
            }
        }



        private void BossAI(ACharacter c, Vector2Int[] directions, IOrderedEnumerable<ACharacter> targets)
        {
            var enemy = (Enemy)c;
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
                if (c is not Enemy || !c.gameObject.activeInHierarchy)
                {
                    continue;
                }
                // Our target is the closest character with a different team than ours
                var targets = _characters
                    .Where(x => (c.EquippedWeapon.IsHeal ? (x.Team == c.Team && x.GetInstanceID() != c.GetInstanceID()) : x.Team != c.Team) && x.gameObject.activeInHierarchy)
                    .OrderBy(x =>
                    {
                        if (x.EquippedWeapon.IsHeal && x.IsHealthFull)
                        {
                            return Vector2.Distance(c.Position, x.Position) * 100f;
                        }
                        return Vector2.Distance(c.Position, x.Position);
                    });

                if (c.CanDoSomething() && targets.Any())
                {
                    if (c.IsBoss)
                    {
                        BossAI(c, directions, targets);
                    }
                    else
                    {
                        NormalAI(c, directions, targets);
                    }
                }
                else
                {
                    ((Enemy)c).AttackCharge = 0;
                }
                c.EndTurn();
            }
        }

        public ACharacter GetCharacterPos(int x, int y)
            => _characters.FirstOrDefault(e => e.Position.x == x && e.Position.y == y);
    }
}
