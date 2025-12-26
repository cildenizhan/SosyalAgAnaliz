using SocialNetworkAnalysis.Models;
using System.Collections.Generic;

namespace SocialNetworkAnalysis.Services
{
    
    public interface IGraphService
    {
        void AddNode(UserNode node);
        void RemoveNode(int nodeId);
        void AddEdge(int sourceId, int targetId);
        void RemoveEdge(int sourceId, int targetId);
        List<UserNode> GetAllNodes();
        List<Edge> GetAllEdges();
        
        [cite_start]
        void RecalculateWeights();
    }
}
