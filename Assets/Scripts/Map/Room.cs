namespace TouhouPrideGameJam4.Map
{
    public record Room
    {
        public Room(int x, int y, string[] data)
            => (X, Y, Data) = (x, y, data);

        public int X { set; get; }
        public int Y { set; get; }
        public string[] Data;
    }
}
