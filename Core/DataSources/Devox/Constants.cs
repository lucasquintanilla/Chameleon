namespace Core.DataSources.Devox
{
    public class Constants
    {
        public const string Domain = "devox.me";
        public const string Url = $"https://{Domain}";
        public const string CdnUrl = "https://cdn.devox.re";
        public const string ApiUrl = $"https://api.{Domain}";
        public const string Enpoint = $"/voxes/getVoxes";
        public const string GetVoxesEnpoint = $"{ApiUrl}{Enpoint}";
        public const string VoxEnpoint = $"{Url}/v/";
    }
}
