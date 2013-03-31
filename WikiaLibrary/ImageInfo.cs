using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiaLibrary
{
    public struct ImageInfo
    {
        public string Name { get; private set; }
        public string Url { get; private set; }

        public ImageInfo(string name, string url)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name");
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("url");

            Name = name.Trim();
            Url = url.Trim();
        }
    }

    public class ImageInfoNameEqualityComparer : IEqualityComparer<ImageInfo>
    {
        public bool Equals(ImageInfo x, ImageInfo y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(ImageInfo obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Name))
                return 0;
            return obj.Name.GetHashCode();
        }
    }
}
