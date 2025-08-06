using Vonage.Common.Monads;

namespace DockerApiSandbox.Api.OperationIdentification;

public static class MaybeExtensions
{
    public static IEnumerable<T> WhereSome<T>(this IEnumerable<Maybe<T>> source) =>
        from maybe in source where maybe.IsSome select maybe.GetUnsafe();
}