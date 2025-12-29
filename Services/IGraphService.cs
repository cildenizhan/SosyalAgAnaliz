using System.Collections.Generic;
using SocialNetworkAnalysis.Models;

namespace SocialNetworkAnalysis.Services
{
    
    public interface IGraphService
    {
        Dictionary<int, Node> Nodes { get; }
        List<Edge> Edges { get; }
        void AddNode(Node node);
        void AddEdge(int sourceId, int targetId);
    }
}