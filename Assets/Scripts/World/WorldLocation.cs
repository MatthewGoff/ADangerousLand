using MessagePack;

namespace ADL.World
{
    [MessagePackObject]
    public struct WorldLocation
    {
        [IgnoreMember]
        public (int X, int Y) Tuple
        {
            get
            {
                return (X, Y);
            }
        }

        [Key(0)] public int X { get; set; }
        [Key(1)] public int Y { get; set; }

        [SerializationConstructor]
        public WorldLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public WorldLocation((int x, int y) tuple)
        {
            X = tuple.x;
            Y = tuple.y;
        }

        public override string ToString()
        {
            return "(" + X.ToString() + "," + Y.ToString() + ")";
        }
    }
}