namespace Rayls.Custody.HSM.Service.Interface.Repositories.Model
{
    public class Token
    {
        public string Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public int Decimals { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }
}
