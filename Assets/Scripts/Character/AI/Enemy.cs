namespace TouhouPrideGameJam4.Character.AI
{
    public class Enemy : ACharacter
    {
        private void Start()
        {
            Init(Team);
        }

        private void Update()
        {
            UpdateC();
        }

        public int AttackCharge = 0;
    }
}
