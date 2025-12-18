namespace OsuCzechiaBot.Jobs.OneTime.Base;

public interface IOneTimeJob
{
    public string Key { get; }
    public DateTimeOffset? RunAt { get; }
    public uint Priority { get; }
    public Task RunAsync(CancellationToken cancellationToken);
}