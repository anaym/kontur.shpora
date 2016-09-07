namespace DLibrary.Graph
{
    public interface IEdge<out TNode>
        where TNode : Node
    {
        TNode From { get; }
        TNode To { get; }
        double Cost { get; }
    }
}