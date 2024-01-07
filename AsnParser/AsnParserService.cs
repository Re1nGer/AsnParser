using AsnParser.Models;

namespace AsnParser;

public class AsnParserService : IAsnParserService
{
    private readonly string _boxKeyWord;
    private readonly string _productKeyWord;

    public AsnParserService(string boxKeyWord, string productKeyWord)
    {
        _boxKeyWord = boxKeyWord;
        _productKeyWord = productKeyWord;
    }
    
    
    //There are multiple ways of saving the data
    //We can save the data once the box is processed 
    //Or we can save the data once the file is processed completely
    //here I assume we should save once the box is processed since the data might exceed available ram
    public async Task<List<Box>> ParseAnsLoadToDbFrom(string filePath)
    {
        var boxCollection = new List<Box>();
        try
        {
            using (var reader = new StreamReader(filePath))
            {
                //we read file line by line since the file might be bigger than available ram
                //if the line exceeds available ram, we can read it as binary in chunks, set the limit for buffer
                //and process chunk by chunk
                //here I assume the line is within the size of available ram
                while (reader.ReadLine() is { } line)
                {
                    if (line.Equals("")) continue;
                    if (line.StartsWith(_boxKeyWord))
                    {
                        //if collection is not empty, it means we have a box to save
                        if (boxCollection.Any())
                        {
                            await Save(boxCollection.Last());
                        }
                        var boxWithHeaders = ParseAnsHeaderLine(line);
                        boxCollection.Add(boxWithHeaders);
                    }
                    else if (line.StartsWith(_productKeyWord))
                    {
                        var lastBox = boxCollection.Last();
                        var content = ParseContentLine(line);
                        lastBox.Contents.Add(content);
                    }
                }

                //at the end, we need to make sure to save the last box
                if (boxCollection.Any())
                {
                    await Save(boxCollection.Last());
                }
            }

            Console.WriteLine("Data loaded into the database successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"The File specified at: {filePath} is not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return boxCollection;
    }

    public Box ParseAnsHeaderLine(string line)
    {
        var box = new Box();
        var headerParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var supplierIdentifier = headerParts[1];
        var cartonBoxIdentifier = headerParts[2];
        box.SupplierIdentifier = supplierIdentifier;
        box.CartonBoxInIdentifier = cartonBoxIdentifier;
        return box;
    }
    public Content ParseContentLine(string line)
    {
        var content = new Content();
        var lineParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var poNumber = lineParts[1];
        var productBarcode = lineParts[2];
        var quantity = int.Parse(lineParts[3]);
        content.Isbn = productBarcode;
        content.PoNumber = poNumber;
        content.Quantity = quantity;
        return content;
    }
    
    public async Task Save(Box box)
    {
        const string dbString = "User ID =postgres;Password=password;Server=localhost;Port=5432;Database=db_test;Pooling=true";
        using (AppContext context = new AppContext(dbString))
        {
            await context.Boxes.AddAsync(box);
            await context.SaveChangesAsync();
        }
    }
}