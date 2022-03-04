namespace Core.Shared
{
    public static class TokenGenerator
    {
        public static string NewToken()
        {
            return new Hash().NewHash(40);
        }
    }
}
