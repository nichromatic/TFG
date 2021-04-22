public class ObjectModelLink
{
    public ObjectModelNode parent;
    public ObjectModelNode child;

    public ObjectModelLink(ObjectModelNode parent, ObjectModelNode child)
    {
        this.parent = parent;
        this.child = child;
    }
}
