namespace ObjectModel
{
    public class ModelLink
    {
        public ModelNode parent;
        public ModelNode child;

        public float chance;

        public ModelLink(ModelNode parent, ModelNode child, float chance = 100f)
        {
            this.parent = parent;
            this.child = child;
            this.chance = chance;
        }
    }
}
