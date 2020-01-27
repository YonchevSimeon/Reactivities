namespace Application.Profiles
{
    using System.Threading.Tasks;

    public interface IProfileReader
    {
        Task<Profile> ReadProfile(string username);
    }
}