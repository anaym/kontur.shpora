namespace DLibrary.Graph
{
    public interface IEdge
    {
        INode From { get; }
        INode To { get; }
    }
}