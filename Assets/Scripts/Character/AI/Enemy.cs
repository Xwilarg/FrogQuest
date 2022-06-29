using System.Linq;
using TouhouPrideGameJam4.Character.Player;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Map;
using TouhouPrideGameJam4.SO.Item;
using UnityEngine;

namespace TouhouPrideGameJam4.Character.AI
{
    public class Enemy : ACharacter
    {
        /// <summary>
        /// Status symbol sprites
        /// </summary>
        private GameObject BindSprite, FreezeSprite, StunSprite, InvulnerableSprite;
        private void Start()
        {
            Init(Team);
        }
        private void Awake() {
            foreach(Transform child in this.transform){
                //if I don't know what the child is, don't change activity
                bool wasActive= child.gameObject.activeSelf;
                child.gameObject.SetActive(false);
                if(child.name.Contains("Bind")){
                    BindSprite=child.gameObject;
                }
                else if (child.name.Contains("Frozen") || child.name.Contains("Freeze")){
                    FreezeSprite = child.gameObject;
                }
                else if (child.name.Contains("Stun")){
                    StunSprite = child.gameObject;
                }
                else if (child.name.Contains("Invulnerability") || child.name.Contains("Invulnerable")){
                    InvulnerableSprite = child.gameObject;
                }else{
                    child.gameObject.SetActive(wasActive);
                }
            }
            
        }
        public override void OnStatusChange()
        {
            base.OnStatusChange();
            BindSprite?.SetActive(_currentEffects.ContainsKey(StatusType.Binded));
            FreezeSprite?.SetActive(_currentEffects.ContainsKey(StatusType.Frozen));
            StunSprite?.SetActive(_currentEffects.ContainsKey(StatusType.Stunned));
            InvulnerableSprite?.SetActive(_currentEffects.ContainsKey(StatusType.Invincible));
        }

        private void Update()
        {
            UpdateC();
        }

        public override void TakeDamage(WeaponInfo weapon, int amount)
        {
            base.TakeDamage(weapon, amount);

            if (IsBoss) // Boss teleport on hit
            {
                int x, y;
                do
                {
                    x = PlayerController.Instance.Position.x;
                    y = PlayerController.Instance.Position.y;
                    if (Random.Range(0, 2) == 0)
                    {
                        x = Random.Range(x - 5, x + 6);
                    }
                    else
                    {
                        y = Random.Range(y - 5, y + 6);
                    }
                } while (Vector2Int.Distance(new(x, y), PlayerController.Instance.Position) <= 2 || !MapManager.Instance.IsTileWalkable(x, y) || TurnManager.Instance.GetCharacterPos(x, y) != null);
                Position = new(x, y);
            }
        }

        public int AttackCharge = 0;
    }
}
