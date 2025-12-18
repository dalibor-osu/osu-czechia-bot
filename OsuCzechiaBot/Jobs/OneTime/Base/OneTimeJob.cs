namespace OsuCzechiaBot.Jobs.OneTime.Base;

public abstract class OneTimeJob : IOneTimeJob
{
    public abstract string Key { get; }
    public virtual DateTimeOffset? RunAt => null;
    public virtual uint Priority => uint.MaxValue;
    public abstract Task RunAsync(CancellationToken cancellationToken);
}