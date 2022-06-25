using Assets.Scripts.Projectile;
using System.Collections;
using TouhouPrideGameJam4.Character;
using TouhouPrideGameJam4.Inventory;
using UnityEngine;

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

        public bool Piercing;
        public bool NoEffectOnBoss;

        public Color StartColor, EndColor;
        public float StartSize, EndSize;
        public int Range = -1;

        public override ItemType Type => ItemType.Spell;

        public override string Description => $"Launch {ProjectileCount} projectiles each doing {DamagePerProjectile} damage";

        public override void DoAction(ACharacter owner)
        {
            if (!owner.hasInfiniteItems){
                owner.RemoveItem(this);
            }
            owner.StartCoroutine(Shoot(owner));
        }

        private IEnumerator Shoot(ACharacter owner)
        {
            for (int i = 0; i < ProjectileCount; i++)
            {
                var go = Instantiate(ProjectilePrefab, (Vector2)owner.Position + new Vector2(Random.Range(-XOffset, XOffset), Random.Range(-YOffset, YOffset)), Quaternion.identity);
                var renderer = go.GetComponent<TrailRenderer>();
                renderer.startColor = StartColor;
                renderer.endColor = EndColor;
                renderer.startWidth = StartSize;
                renderer.endWidth = EndSize;
                var proj = go.GetComponent<DamageProjectile>();
                proj.Damage = DamagePerProjectile;
                proj.CurrentPos = owner.Position;
                proj.NoEffectOnBoss = NoEffectOnBoss;
                proj.DestroyAfterFirstHit = !Piercing;
                proj.Owner = owner.GetInstanceID();
                proj.Range = Range;
                var direction = owner.RelativeDirection;
                go.GetComponent<Rigidbody2D>().AddForce((Vector2)direction * Speed);
                Destroy(go, 10f);
                yield return new WaitForSeconds(DelayBetweenProjectiles);
            }
        }
    }
}