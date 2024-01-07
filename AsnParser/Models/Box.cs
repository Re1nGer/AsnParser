namespace AsnParser.Models;

//I wouldn't use domain class directly, probably would introduce DTOs with automapper 
public class Box
{
    public Guid Id { get; set; }
    public string SupplierIdentifier { get; set; }
    public string CartonBoxInIdentifier { get; set; }
    //In the example code, Contents property is set to be IReadOnlyCollection
    //I modified it to be a regular mutable list 
    public List<Content> Contents { get; set; } = new ();
}