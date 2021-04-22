public class ObjectModelLink
{
    public ObjectModelNode parent;
    public ObjectModelNode child;

    public float chance;

    public ObjectModelLink(ObjectModelNode parent, ObjectModelNode child, float chance = 100f)
    {
        this.parent = parent;
        this.child = child;
        this.chance = chance;
    }
}
