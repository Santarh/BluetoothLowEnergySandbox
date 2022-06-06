namespace BlackmagicCameraControlProtocol
{
    internal static class BlackmagicExtensions
    {
        public static byte[] AsBlackmagicSignedFixedPoint16(this double val)
        {
            var x = (ushort) Math.Floor(val * Math.Pow(2, 11));
            return BitConverter.GetBytes(x);
        }
        
    }
}