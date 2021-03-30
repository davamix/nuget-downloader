using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetDownloader.Events
{
    public class PackageDownloadedEvent : PubSubEvent<string>
    {
    }
}
