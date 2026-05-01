namespace Skote.Helpers;

public interface IRemoteNotifyClient
{
    Task SendTestAsync(CancellationToken ct = default);
}
