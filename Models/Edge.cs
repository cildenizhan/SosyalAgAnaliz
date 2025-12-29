namespace SocialNetworkAnalysis.Models
{
    public class Edge
    {
        public Node Source { get; set; }
        public Node Target { get; set; }
        public double Weight { get; set; } 

        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;
            Weight = 0;
        }
    }
}