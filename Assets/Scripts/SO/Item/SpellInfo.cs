using System.Collections;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Game;
using TouhouPrideGameJam4.Map;
using UnityEngine;
using static UnityEngine.UIElements.NavigationMoveEvent;

namespace TouhouPrideGameJam4.SO.Item
{
    [CreateAssetMenu(menuName = "ScriptableObject/Item/SpellInfo", fileName = "SpellInfo")]
    public class SpellInfo : ConsumableInfo
    {
        /// <summary>
        /// Actual projectile that will spawn
        /// </summary>
        [Header("Spell info")]
        public GameObject ProjectilePrefab;

        /// <summary>
        /// Speed of the projectile
        /// </summary>
        public float Speed;

        /// <summary>
        /// Number of projectile
        /// </summary>
        public float ProjectileCount;

        /// <summary>
        /// Damage each projectile will do
        /// </summary>
        public int DamagePerProjectile;

        /// <summary>
        /// Delay between each projectile is spawn
        /// </summary>
        public float DelayBetweenProjectiles;

        /// <summary>
        /// Random offset applied to spawn position
        /// </summary>
        public float XOffset, YOffset;

        public override string Description => $"Launch {ProjectileCount} projectiles each doing {DamagePerProjectile} damages";

        public override void DoAction(ACharacter owner)
        {
            owner.RemoveItem(this);
            owner.StartCoroutine(Shoot(owner));
        }

        private IEnumerator Shoot(ACharacter owner)
        {
            for (int i = 0; i < ProjectileCount; i++)
            {
                var go = Instantiate(ProjectilePrefab, (Vector2)owner.Position + new Vector2(Random.Range(-XOffset, XOffset), Random.Range(-YOffset, YOffset)), Quaternion.identity);
                var direction = owner.Direction switch
                {
                    Direction.Left => Vector2Int.left,
                    Direction.Right => Vector2Int.right,
                    Direction.Up => Vector2Int.up,
                    Direction.Down => Vector2Int.down,
                    _ => throw new System.NotImplementedException()
                };
                int currX = owner.Position.x, currY = owner.Position.y;
                do
                {
                    currX += direction.x;
                    currY += direction.y;

                    TurnManager.Instance.TryAttackCharacter(currX, currY, DamagePerProjectile);
                } while (MapManager.Instance.IsTileWalkable(currX, currY));
                go.GetComponent<Rigidbody2D>().AddForce((Vector2)direction * Speed);
                Destroy(go, 10f);
                yield return new WaitForSeconds(DelayBetweenProjectiles);
            }
        }
    }
}