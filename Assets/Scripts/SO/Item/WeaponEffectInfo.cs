using System;
using TouhouPrideGameJam4.Character;

namespace TouhouPrideGameJam4.SO.Item
{
    [Serializable]
    public class WeaponEffectInfo
    {
        public StatusType Effect;
        public int TurnCount;
        public int Chance;
    }
}
