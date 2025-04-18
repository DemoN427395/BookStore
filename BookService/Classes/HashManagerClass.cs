using System.Security.Cryptography;

namespace BookService.Classes;

public class HashManagerClass
{
    public async Task<string> ComputeHashFromMemoryStreamAsync(MemoryStream memoryStream)
    {
        try
        {
            memoryStream.Position = 0;

            using (var sha256 = SHA256.Create())
            {

                byte[] hashBytes = await sha256.ComputeHashAsync(memoryStream);

                string hashString = BitConverter.ToString(hashBytes)
                    .Replace("-", "")
                    .ToLowerInvariant();
                return hashString;
            }
        }
        catch(Exception ex)
        {
            throw new Exception("Error computing hash", ex);
        }
    }
}