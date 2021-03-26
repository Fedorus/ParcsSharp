using Parcs;

namespace RelAlgebra
{
    public class RelCommand
    {
        public CommandType CommandType {get; set; }
        public string Expression { get; set; }
        public Channel DataFrom { get; set; }
        public Channel DataTo { get; set; }
        public RelInfo RelInfo { get; set; }
    }
}