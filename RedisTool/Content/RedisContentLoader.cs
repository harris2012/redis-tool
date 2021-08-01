using FirstFloor.ModernUI.Windows;
using RedisTool.Content.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisTool.Content
{
    public class RedisContentLoader : DefaultContentLoader
    {
        protected override object LoadContent(Uri uri)
        {
            return new AboutPage();
        }
    }
}
