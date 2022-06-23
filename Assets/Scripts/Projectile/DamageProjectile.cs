using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Map;
using UnityEngine;

namespace Assets.Scripts.Projectile
{
    public class DamageProjectile : MonoBehaviour
    {
        public bool DestroyAfterFirstHit { set; get; }
        public Vector2Int CurrentPos { set; get; }
        public int Damage { set; get; }
        public bool NoEffectOnBoss { set; get; }

        private void Update()
        {
            var pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            if (pos != CurrentPos)
            {
                CurrentPos = pos;
                var target = TurnManager.Instance.GetCharacterPos(CurrentPos.x, CurrentPos.y);
                if (target != null)
                {
                    target.TakeDamage(null, target.IsBoss && NoEffectOnBoss ? 0 : Damage);
                    if (DestroyAfterFirstHit)
                    {
                        Destroy(gameObject);
                    }
                }
                else if (!MapManager.Instance.IsTileWalkable(CurrentPos.x, CurrentPos.y))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
