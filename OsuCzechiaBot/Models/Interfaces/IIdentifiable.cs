namespace OsuCzechiaBot.Models.Interfaces;

public interface IIdentifiable<TId> where TId : IEquatable<TId>
{
    public TId Id { get; set; }
}

public interface IIdentifiable : IIdentifiable<ulong>;