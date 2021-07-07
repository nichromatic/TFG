using System;

namespace ObjectModel
{
    [Serializable]
    public class LinkData
    {
        public string parentNodeID;
        public string childNodeID;

        public string portName;
        public float chance;
    }
}