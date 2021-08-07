namespace ObjectModel
{
    public class ModelLink
    {
        public ModelBaseNode parent;
        public ModelBaseNode child;

        public float chance;

        public ModelLink(ModelBaseNode parent, ModelBaseNode child, float chance = 100f)
        {
            this.parent = parent;
            this.child = child;
            this.chance = chance;
        }
    }
}
