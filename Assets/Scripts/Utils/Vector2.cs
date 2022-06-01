namespace TouhouPrideGameJam4.Utils
{
    public class Vector2<T>
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public T X { private set; get; }
        public T Y { private set; get; }
    }
}
