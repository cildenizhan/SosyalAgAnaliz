namespace SocialNetworkAnalysis.Models
{
    public class Edge
    {
        public UserNode Source { get; set; }
        public UserNode Target { get; set; }
        public double Weight { get; set; } 

        public Edge(UserNode source, UserNode target)
        {
            Source = source;
            Target = target;
            
            Weight = 0;
        }
    }
}