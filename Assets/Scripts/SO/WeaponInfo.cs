using UnityEngine;

namespace TouhouPrideGameJam4.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/WeaponInfo", fileName = "WeaponInfo")]
    public class WeaponInfo : ScriptableObject
    {
        public string Name;
        public int Damage;
    }
}