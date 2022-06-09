namespace BlackmagicCameraControlProtocol;

internal static class BlackmagicExtensions
{
    public static byte[] AsBlackmagicSignedFixedPoint16(this double val)
    {
        var x = (short)Math.Floor(val * Math.Pow(2, 11));
        return BitConverter.GetBytes(x);
    }

    public static short FromBlackmagicSignedInt16(this byte[] data)
    {
        return BitConverter.ToInt16(data);
    }
}