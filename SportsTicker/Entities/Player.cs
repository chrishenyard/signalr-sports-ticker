namespace SportsTicker.Entities
{
    public class Player
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Stats Stats { get; set; }
        public string Headshot { get; set; }
    }
}
