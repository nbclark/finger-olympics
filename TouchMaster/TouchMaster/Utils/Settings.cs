using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;

namespace MobileSrc.FantasticFingerFun
{
    public class Settings
    {
        private const string _settingsStorage = @"\settings.xml";
        private static XmlSerializer _settingsSerializer = new XmlSerializer(typeof(Settings));
        private static Settings _instance = null;

        public static Settings Instance
        {
            get
            {
                if (null == _instance)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        try
                        {
                            using (Stream stream = store.OpenFile(_settingsStorage, FileMode.Open))
                            {
                                using (XmlReader reader = XmlReader.Create(stream))
                                {
                                    _instance = (Settings)_settingsSerializer.Deserialize(reader);
                                }
                            }
                        }
                        catch
                        {
                            // failed reading cached file
                            // assuming this is first launch : return fake data
                            _instance = new Settings();
                        }
                    }
                }
                return _instance;
            }
        }

        public Settings()
        {
            this.ArrowsEnabled = false;
            this.SoundsEnabled = false;
            this.GamerTag = string.Empty;
        }

        public bool ArrowsEnabled
        {
            get;
            set;
        }

        public bool SoundsEnabled
        {
            get;
            set;
        }

        public string GamerTag
        {
            get;
            set;
        }

        public void Save()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // create a new file (overwrite existing)
                using (IsolatedStorageFileStream file = store.CreateFile(_settingsStorage))
                {
                    _settingsSerializer.Serialize(file, this);
                }
            }
        }
    }
}
