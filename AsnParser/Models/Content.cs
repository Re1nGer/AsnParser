namespace AsnParser.Models;

public class Content
{
    public int Id { get; set; }
    public string PoNumber { get; set; }
    public string Isbn { get; set; }
    public int Quantity { get; set; }
    public Guid BoxId { get; set; }
    public Box Box { get; set; }
}
