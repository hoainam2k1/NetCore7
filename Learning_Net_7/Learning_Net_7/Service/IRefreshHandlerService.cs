namespace Learning_Net_7.Service
{
    public interface IRefreshHandlerService
    {
        Task<string> GenerateToken(string userName);
    }
}
