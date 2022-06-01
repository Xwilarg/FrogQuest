using static UnityEngine.UIElements.NavigationMoveEvent;

namespace TouhouPrideGameJam4.Map
{
    public record Door
    {
        public Door(int x, int y, Direction direction)
            => (X, Y, Direction) = (x, y, direction);

        public int X { get; set; }
        public int Y { get; set; }
        public Direction Direction { get; set; }
    }
}
