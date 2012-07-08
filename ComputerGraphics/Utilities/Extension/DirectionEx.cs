namespace Utilities.Extension
{
    using Enumerations;

    public static class DirectionEx
    {
        public static int Signal(this Direction t)
        {
            if (t == Direction.Positive)
                return 1;

            return -1;
        }
    }
}