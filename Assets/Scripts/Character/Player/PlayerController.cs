using System.Collections.Generic;
using TouhouPrideGameJam4.Dialog;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Game.Persistency;
using TouhouPrideGameJam4.SO.Character;
using TouhouPrideGameJam4.SO.Item;
using TouhouPrideGameJam4.Sound;
using TouhouPrideGameJam4.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TouhouPrideGameJam4.Character.Player
{
    public class PlayerController : ACharacter
    {
        public static PlayerController Instance { get; private set; }

        [SerializeField]
        private AudioClip[] _stepSound;
        private int _stepIndex;

        private PlayerInput _input;

        private FollowerInfo _followerInfo;

        public int Energy { private set; get; }

        private Vector2Int _walkDirection;

        private void Awake()
        {
            Instance = this;
            _input = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "Main") // Outside of main scene we only use this object for the VN controls
            { // TODO: Move into separate class?
                if (PersistencyManager.Instance.Follower != null)
                {
                    SetFollower(PersistencyManager.Instance.Follower);
                    if (PersistencyManager.Instance.Follower.Type == FollowerType.Reimu)
                    {
                        _baseHealthMult = 1.2f;
                    }
                }
                if (PersistencyManager.Instance.StoryProgress == StoryProgress.EndQuest)
                {
                    PersistencyManager.Instance.StoryProgress = StoryProgress.Quest;
                    PersistencyManager.Instance.QuestStatus = QuestStatus.NotStarted;
                    PersistencyManager.Instance.QuestProgress = 0;
                }
                if (PersistencyManager.Instance.EasyMode)
                {
                    _baseHealthMult *= 3f;
                }
                Init(Team.Allies);
            }
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "Main")
            {
                UpdateC();
                if (_didReachPosition && _walkDirection != Vector2Int.zero)
                {
                    if (TurnManager.Instance.MovePlayer(_walkDirection.x, _walkDirection.y, true))
                    {
                        OnDoneWalking();
                    }
                    else
                    {
                        _walkDirection = Vector2Int.zero;
                    }
                }
            }
        }

        public void IncreaseEnergy(int value)
        {
            Energy += value;
            UIManager.Instance.SetEnergyText(Energy);
        }

        private void SetFollower(FollowerInfo follower)
        {
            _followerInfo = follower;
            if (follower.Type == FollowerType.Aya)
            {
                UIManager.Instance.SetFollowerAya();
            }
            else if (follower.Type == FollowerType.Reimu)
            {
                UIManager.Instance.SetFollowerReimu();
            }
            else
            {
                throw new System.NotImplementedException($"Unknown character {follower.Type}");
            }
        }

        public override void OnStatusChange()
        {
            base.OnStatusChange();
            UIManager.Instance.UpdateStatus(_currentEffects);
        }

        public override StatusType[] CurrentEffects
        {
            get
            {
                var effects = base.CurrentEffects;
                if (_followerInfo != null)
                {
                    return new List<StatusType>(effects)
                    {
                        _followerInfo.Status
                    }.ToArray();
                }
                return effects;
            }
        }

        public override void UpdateInventoryDisplay()
        {
            UIManager.Instance.ShortcutEquipped.sprite = EquippedWeapon.Sprite;
            UIManager.Instance.ShortcutEquipped.color = UIManager.Instance.ShortcutEquipped.sprite == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
            int index = 0;
            foreach (var btn in UIManager.Instance.ShortcutInventory)
            {
                btn.Clear();
            }
            foreach (var item in _items)
            {
                if (item == EquippedWeapon)
                {
                    continue;
                }

                UIManager.Instance.ShortcutInventory[index].SetContent(item);
                index++;
                if (index == UIManager.Instance.ShortcutInventory.Length)
                {
                    break;
                }
            }
        }

        public override void TakeDamage(WeaponInfo weapon, int amount)
        {
            base.TakeDamage(weapon, amount);

            UIManager.Instance.SetHealth(_health / (float)_info.BaseHealth);
        }

        private void OnDoneWalking()
        {
            _didReachPosition = false;
            _stepIndex++;
            if (_stepIndex == _stepSound.Length)
            {
                _stepIndex = 0;
            }
            SoundManager.Instance.PlayFootstepsClip(_stepSound[_stepIndex]);
            UIManager.Instance.UpdateUIOnNewTile();
        }

        public void EnableVNController()
        {
            _input.SwitchCurrentActionMap("VN");
        }

        public void EnableRPGController()
        {
            _input.SwitchCurrentActionMap("RPG");
        }

        public void OnNextDialog(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                StoryManager.Instance.ShowNextDialogue();
            }
        }

        public void OnSkipDialogs(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                StoryManager.Instance.ToggleSkipDialogs();
            }
        }

        public void GoBackDialogue(InputAction.CallbackContext value)
        {
            if (value.performed && value.ReadValue<Vector2>().y > 0f)
            {
                StoryManager.Instance.GoBack();
            }
        }

        private bool _waitingForNextPress;
        public void OnMovement(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();
            if (mov.x != 0f || mov.y != 0f)
            {
                if (value.phase != InputActionPhase.Started && _waitingForNextPress)
                {
                    return;
                }
                if (Mathf.Abs(mov.x) > Mathf.Abs(mov.y))
                {
                    _walkDirection = new(mov.x > 0 ? 1 : -1, 0);
                    if (_didReachPosition)
                    {
                        if (TurnManager.Instance.MovePlayer(_walkDirection.x, _walkDirection.y, false))
                        {
                            OnDoneWalking();
                        }
                        else
                        {
                            _waitingForNextPress = true;
                            _walkDirection = Vector2Int.zero;
                        }
                    }
                }
                else
                {
                    _walkDirection = new(0, mov.y > 0 ? 1 : -1);
                    if (_didReachPosition)
                    {
                        if (TurnManager.Instance.MovePlayer(_walkDirection.x, _walkDirection.y, false))
                        {
                            OnDoneWalking();
                        }
                        else
                        {
                            _waitingForNextPress = true;
                            _walkDirection = Vector2Int.zero;
                        }
                    }
                }
            }
            else
            {
                _walkDirection = Vector2Int.zero;
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                UIManager.Instance.UseCurrent();
            }
        }

        public void OnMenu(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                UIManager.Instance.TogglePauseMenu();
            }
        }

        public void OnWait(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                TurnManager.Instance.PlayEnemyTurn();
            }
        }

        public void OnDropTake(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                UIManager.Instance.DropTake();
            }
        }

        public void OnShortcut1(InputAction.CallbackContext value) => OnShortcut(value, 0);
        public void OnShortcut2(InputAction.CallbackContext value) => OnShortcut(value, 1);
        public void OnShortcut3(InputAction.CallbackContext value) => OnShortcut(value, 2);
        public void OnShortcut4(InputAction.CallbackContext value) => OnShortcut(value, 3);
        public void OnShortcut5(InputAction.CallbackContext value) => OnShortcut(value, 4);
        public void OnShortcut6(InputAction.CallbackContext value) => OnShortcut(value, 5);
        public void OnShortcut7(InputAction.CallbackContext value) => OnShortcut(value, 6);
        public void OnShortcut8(InputAction.CallbackContext value) => OnShortcut(value, 7);
        public void OnShortcut9(InputAction.CallbackContext value) => OnShortcut(value, 8);
        public void OnShortcut10(InputAction.CallbackContext value) => OnShortcut(value, 9);

        private void OnShortcut(InputAction.CallbackContext value, int index)
        {
            if (value.performed)
            {
                UIManager.Instance.ShortcutTarget = UIManager.Instance.ShortcutInventory[index];
            }
        }
    }
}
