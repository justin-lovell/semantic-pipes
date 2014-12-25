namespace SemanticPipes
{
    internal interface IRegistryMediator
    {
        void AppendObserver(ISemanticRegistryObserver observer);
        void AppendPackage(PipeOutputPackage package);
        ISolver CreateSolver();
    }
}