namespace Application.Interfaces
{
    using Domain;
    
    public interface IJwtGenerator
    {
         string CreateToken(AppUser user);
    }
}