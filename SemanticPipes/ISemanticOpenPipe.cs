using System.Threading.Tasks;

namespace SemanticPipes
{
    public interface ISemanticOpenPipe
    {
        Task<TDestination> Output<TDestination>();
    }
}