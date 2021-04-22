using System.Collections.Generic;

public class ProceduralObject
{
    public ProceduralObjectNode rootNode;
    public List<ProceduralObjectNode> allNodes;

    public ProceduralObject(ObjectModel model)
    {
        Generate(model);
    }

    private void Generate(ObjectModel model)
    {
        rootNode = new ProceduralObjectNode(null, model.rootNode, this);
        rootNode.GenerateChildren(model.rootNode);
    }
}
