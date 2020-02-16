namespace Data.Services.User
{
    public interface IUserServices
    {
        bool ValidateUser(string username, string password);
    }
}