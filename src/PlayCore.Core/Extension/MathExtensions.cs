namespace PlayCore.Core.Extension
{
    public static class MathExtensions
    {
        public static int UpDivision(int x, int y)
        {
            if (y == 0)
                return 0;
            decimal z = x / (decimal)y;
            return z > (int)z ? (int)(z + 1) : (int)z;
        }
    }
}
