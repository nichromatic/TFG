using System.Collections.Generic;
using ObjectModel;

namespace ProceduralGenerator
{
    public class ProceduralObject
    {
        public ProceduralObjectNode rootNode;
        public List<ProceduralObjectNode> allNodes;

        public ProceduralObject(Model model)
        {
            Generate(model);
        }

        private void Generate(Model model)
        {
            rootNode = new ProceduralObjectNode(null, model.rootNode, this);
            rootNode.GenerateChildren(model.rootNode);
        }
    }
}
