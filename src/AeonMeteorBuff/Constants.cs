using System.Reflection;

namespace AeonMeteorBuff
{
    public static class Constants
    {
        public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                             BindingFlags.Instance | BindingFlags.DeclaredOnly;
    }
}