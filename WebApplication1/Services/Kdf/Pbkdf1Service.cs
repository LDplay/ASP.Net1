using WebApplication1.Services.Hash;

namespace WebApplication1.Services.Kdf
{
    // Згідно з п. 5.1 RFC 2898
    public class Pbkdf1Service(IHashService hashService) : IKdfService
    {
        private readonly IHashService _hashServices = hashService;

        public string DerivedKey(string password, string salt)
        {
            String t1 = _hashServices.Digest(password + salt);
            String t2 = _hashServices.Digest(t1);
            return t2;
        }
    }
}
