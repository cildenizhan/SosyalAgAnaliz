using SocialNetworkAnalysis.Models;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Services
{
    public class GraphService : IGraphService
    {
        
        public Graph GraphModel { get; private set; } = new Graph();

        
        public Dictionary<int, Node> Nodes => GraphModel.Nodes;
        public List<Edge> Edges => GraphModel.Edges;

        public void AddNode(Node node)
        {
            if (!GraphModel.Nodes.ContainsKey(node.Id))
            {
                GraphModel.Nodes.Add(node.Id, node);
            }
        }

        public void AddEdge(int sourceId, int targetId)
        {
            if (GraphModel.Nodes.ContainsKey(sourceId) && GraphModel.Nodes.ContainsKey(targetId))
            {
                var source = GraphModel.Nodes[sourceId];
                var target = GraphModel.Nodes[targetId];

                
                bool exists = GraphModel.Edges.Any(e => 
                    (e.Source == source && e.Target == target) || 
                    (e.Source == target && e.Target == source));

                if (!exists)
                {
                    GraphModel.Edges.Add(new Edge(source, target));
                }
            }
        }
    }
}