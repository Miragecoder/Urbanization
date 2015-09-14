namespace Mirage.Urbanization.Web
{
    public struct ClientZonePoint
    {
        public int x { get; set; }
        public int y { get; set; }
        public string GetIdentityString() => $"{x}_{y}";
    }
}