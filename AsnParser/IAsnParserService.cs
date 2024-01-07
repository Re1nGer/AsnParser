using AsnParser.Models;

namespace AsnParser;

//In case of message structure is different, we may create another implementation and extend this interface
public interface IAsnParserService
{
    //The structure of Box and Content can be encapsulated into an interface and further extended if new properties are added
    Task<List<Box>> ParseAnsLoadToDbFrom(string filePath);
    Box ParseAnsHeaderLine(string line);
    Content ParseContentLine(string line);
    Task Save(Box box);
}