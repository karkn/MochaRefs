using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Integration.Pocket
{
    public class PocketItem
    {
        public Uri Uri { get; private set; }
        public string Title { get; private set; }

        public PocketItem(Uri uri, string title)
        {
            Uri = uri;
            Title = title;
        }
    }
}
