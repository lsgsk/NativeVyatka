namespace Abstractions.Interfaces.Utilities
{
    public interface IMd5HashGenerator
    {
        string GenerateHash(string value);
    }
}
