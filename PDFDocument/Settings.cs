using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace PDFDocument
{
    public enum ProgramState
    {
        Start,
        Read,
        Complete
    }

    [DataContract]
    public class Settings
    {
        [DataMember]
        public int ActivePage;

        [DataMember]
        public string ActiveFileName;

        static string path = "Config.json";

        public ProgramState State;

        public bool CheckFile
        {
            get
            {
                return File.Exists(path);
            }
        }        

        private static Settings instance;

        public static Settings Instance
        {
            get
            {
                DataContractJsonSerializer serializer;
                FileStream fileStream;
                if (instance == null)
                {
                    serializer = new DataContractJsonSerializer(typeof(Settings));
                    if (File.Exists(path))
                    {
                        fileStream = new FileStream(path, FileMode.Open);
                        instance = (Settings)(serializer.ReadObject(fileStream));
                    }
                    else
                    {
                        instance = new Settings();
                    }
                }

                return instance;
            }
        }

        private Settings()
        {
            
        }

        public void Save()
        {
            DataContractJsonSerializer serializer;
            FileStream fileStream;

            fileStream = new FileStream(path, FileMode.Create);
            serializer = new DataContractJsonSerializer(typeof(Settings));
            serializer.WriteObject(fileStream, this);

        }


        
    }


}
