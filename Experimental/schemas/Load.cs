using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ZBrad.NLogEtw.Schemas
{
    public static class Load
    {
        public const string DefaultManifest = "ZBrad.NLogEtw.Resources.NLogEtwman.xml";

        static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        public static InstrumentationManifestType EtwManifest(string path)
        {
            return load<InstrumentationManifestType>(path);
        }

        public static InstrumentationManifestType EtwManifest(StreamReader sr)
        {
            return load<InstrumentationManifestType>(sr);
        }

        static T load<T>(string path) where T : class
        {
            using (var sr = new StreamReader(path))
            {
                return load<T>(sr);
            }
        }

        static T load<T>(StreamReader sr) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                var manifest = (T)serializer.Deserialize(sr);
                return manifest;
            }
            catch (Exception e)
            {
                log.Error("Invalid manifest file format, exception: {0}", e.Message);
                return null;
            }
        }

    }
}
