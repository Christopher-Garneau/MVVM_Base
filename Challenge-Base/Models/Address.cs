namespace Challenge_Base.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
