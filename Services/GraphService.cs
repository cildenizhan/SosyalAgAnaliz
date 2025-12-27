using SocialNetworkAnalysis.Models;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Services
{
    public class GraphService
    {
         
        public Dictionary<int, UserNode> Nodes { get; private set; }
        public List<Edge> Edges { get; private set; }

        public GraphService()
        {
            Nodes = new Dictionary<int, UserNode>();
            Edges = new List<Edge>();
        }

        public void AddNode(UserNode node)
        {
            if (!Nodes.ContainsKey(node.Id))
            {
                Nodes.Add(node.Id, node);
            }
        }

        public void AddEdge(int sourceId, int targetId)
        {
            if (Nodes.ContainsKey(sourceId) && Nodes.ContainsKey(targetId))
            {
                var source = Nodes[sourceId];
                var target = Nodes[targetId];

                
                bool exists = Edges.Any(e => 
                    (e.Source.Id == sourceId && e.Target.Id == targetId) || 
                    (e.Source.Id == targetId && e.Target.Id == sourceId));

                if (!exists)
                {
                    Edges.Add(new Edge(source, target));
                }
            }
        }
    }
}