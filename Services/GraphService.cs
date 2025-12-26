using SocialNetworkAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkAnalysis.Services
{
    public class GraphService : IGraphService
    {
        
        private Dictionary<int, UserNode> _nodes;
        private List<Edge> _edges;

        public GraphService()
        {
            _nodes = new Dictionary<int, UserNode>();
            _edges = new List<Edge>();
        }

        public void AddNode(UserNode node)
        {
            if (!_nodes.ContainsKey(node.Id))
            {
                _nodes.Add(node.Id, node);
            }
        }

        public void RemoveNode(int nodeId)
        {
            if (_nodes.ContainsKey(nodeId))
            {
                
                _edges.RemoveAll(e => e.Source.Id == nodeId || e.Target.Id == nodeId);
                _nodes.Remove(nodeId);
            }
        }

        public void AddEdge(int sourceId, int targetId)
        {
            if (_nodes.ContainsKey(sourceId) && _nodes.ContainsKey(targetId))
            {
                
                var source = _nodes[sourceId];
                var target = _nodes[targetId];

                
                bool exists = _edges.Any(e => 
                    (e.Source.Id == sourceId && e.Target.Id == targetId) || 
                    (e.Source.Id == targetId && e.Target.Id == sourceId));

                if (!exists)
                {
                    _edges.Add(new Edge(source, target));
                }
            }
        }

        public void RemoveEdge(int sourceId, int targetId)
        {
             _edges.RemoveAll(e => 
                    (e.Source.Id == sourceId && e.Target.Id == targetId) || 
                    (e.Source.Id == targetId && e.Target.Id == sourceId));
        }

        public List<UserNode> GetAllNodes() => _nodes.Values.ToList();
        public List<Edge> GetAllEdges() => _edges;

        public void RecalculateWeights()
        {
            
        }
    }
}