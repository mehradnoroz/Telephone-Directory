namespace TelephoneDirectory.Models.contact
{
    public class Contact
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Mobile { get; set; }
        public DateOnly? BirthDate { get; set; }
        public byte[]? Photo { get; set; }
    }
}