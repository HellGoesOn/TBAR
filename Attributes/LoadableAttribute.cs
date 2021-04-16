using System;

namespace TBAR.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LoadableAttribute : Attribute
    {
        public LoadableAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
