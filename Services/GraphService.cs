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
                    var edge = new Edge(source, target);
                    GraphModel.Edges.Add(edge);
                    source.ConnectionCount++;
                    target.ConnectionCount++;
                }
            }
        }

        public void RemoveNode(int nodeId)
        {
            if (GraphModel.Nodes.ContainsKey(nodeId))
            {
                var node = GraphModel.Nodes[nodeId];
                var edgesToRemove = GraphModel.Edges
                    .Where(e => e.Source == node || e.Target == node)
                    .ToList();

                foreach (var edge in edgesToRemove)
                {
                    if (edge.Source == node) edge.Target.ConnectionCount--;
                    else edge.Source.ConnectionCount--;

                    GraphModel.Edges.Remove(edge);
                }

                GraphModel.Nodes.Remove(nodeId);
            }
        }

        public void RemoveEdge(int sourceId, int targetId)
        {
             var edge = GraphModel.Edges.FirstOrDefault(e => 
                (e.Source.Id == sourceId && e.Target.Id == targetId) || 
                (e.Source.Id == targetId && e.Target.Id == sourceId));

            if (edge != null)
            {
                edge.Source.ConnectionCount--;
                edge.Target.ConnectionCount--;
                GraphModel.Edges.Remove(edge);
            }
        }
    }
}