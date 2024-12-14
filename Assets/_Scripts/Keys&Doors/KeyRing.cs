using System.Collections.Generic;


public static class KeyRing
{
    private static readonly HashSet<int> keyIDs = new HashSet<int>();

    public static void AddKey(int keyID)
    {
        keyIDs.Add(keyID);
    }

    public static bool HasKey(int doorID)
    {
        return keyIDs.Contains(doorID);
    }

    public static void ClearKeyRing()
    {
        keyIDs.Clear();
    }
}
