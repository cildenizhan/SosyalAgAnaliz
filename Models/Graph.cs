using System.Collections.Generic;

namespace SocialNetworkAnalysis.Models
{
    
    public class Graph
    {
        public Dictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();
    }
}