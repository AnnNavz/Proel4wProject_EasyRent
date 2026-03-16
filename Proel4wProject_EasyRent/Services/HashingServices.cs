using System.Security.Cryptography;
using System.Text;

namespace Proel4wProject_EasyRent.Services
{
    public class HashingServices
    {

        public static string HashData(string usersData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(usersData);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder1 = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder1.Append(hashBytes[i].ToString("x2"));
                }
                return builder1.ToString();
            }
        }
    }
}
