using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICP
{
    //
    public class MyConfig
    {
        public int PaletteDistance;
        public int MiddleDistance;
        public int LeftArmDistance;
        public int GeneratePointInterval;
        public int ContextUpdateInterval;
        public int SwipeInterval;
        public int CircleInterval;
        public int SteadyInterval;
        public int PushInterval;
        public int PushStableInterval;
    }

    //
    public class MyConfigReader
    {
        static public MyConfig Read(String fileName)
        {
            MyConfig config = new MyConfig();

            System.Xml.Serialization.XmlSerializer reader
                = new System.Xml.Serialization.XmlSerializer(config.GetType());

            System.IO.StreamReader file
                = new System.IO.StreamReader(fileName);

            config = (MyConfig)reader.Deserialize(file);

            file.Close();
            file.Dispose();

            return config;
        }
    }

    //
    public class MyConfigWriter
    {
        static public void Write(MyConfig config, String fileName)
        {
            System.Xml.Serialization.XmlSerializer writer
                = new System.Xml.Serialization.XmlSerializer(config.GetType());

            System.IO.StreamWriter file
                 = new System.IO.StreamWriter(fileName);
            writer.Serialize(file, config);

            file.Flush();
            file.Close();
            file.Dispose();
        }
    }

}
