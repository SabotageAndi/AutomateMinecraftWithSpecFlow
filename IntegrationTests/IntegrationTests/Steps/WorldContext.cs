using System.Collections.Generic;

namespace IntegrationTests.Steps
{
    public class WorldContext
    {
        public int YLevel { get; set; }

        public Dictionary<string, Block> Blocks { get; set; } = new Dictionary<string, Block>();
    }
}