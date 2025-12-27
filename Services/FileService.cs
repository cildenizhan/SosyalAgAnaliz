using SocialNetworkAnalysis.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows; 

namespace SocialNetworkAnalysis.Services
{
    public class FileService
    {
        public GraphService LoadGraph(string filePath)
        {
            var graphService = new GraphService();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Dosya bulunamadı: " + filePath);
                return graphService; 
            }

            var lines = File.ReadAllLines(filePath);

            
            foreach (var line in lines.Skip(1)) 
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(';'); 
                
                try
                {
                    
                    int id = int.Parse(parts[0]);
                    string name = parts[1];
                    
                    double activity = double.Parse(parts[2].Replace('.', ','));
                    double interaction = double.Parse(parts[3].Replace('.', ','));
                    int connectionCount = int.Parse(parts[4]);

                    var node = new UserNode 
                    { 
                        Id = id, 
                        Name = name, 
                        Activity = activity, 
                        Interaction = interaction, 
                        ConnectionCount = connectionCount 
                    };

                    graphService.AddNode(node);
                }
                catch
                {
                    continue; 
                }
            }

            
            foreach (var line in lines.Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(';');
                int sourceId = int.Parse(parts[0]);

                
                if (parts.Length > 5 && !string.IsNullOrEmpty(parts[5]))
                {
                    var neighborIds = parts[5].Split(',').Select(int.Parse);
                    foreach (var targetId in neighborIds)
                    {
                        graphService.AddEdge(sourceId, targetId);
                    }
                }
            }

            return graphService;
        }
    }
}