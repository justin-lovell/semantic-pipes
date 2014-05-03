namespace SemanticPipes
{
    public interface ISemanticBroker
    {
        ISemanticOpenPipe On<TSource>(TSource source) where TSource : class;
    }
}