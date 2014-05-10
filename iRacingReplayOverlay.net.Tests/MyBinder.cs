using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace iRacingReplayOverlay.net.Tests
{
    class MyBinder : SerializationBinder
    {
        private SerializationBinder serializationBinder;

        public MyBinder(SerializationBinder serializationBinder)
        {
            this.serializationBinder = serializationBinder;
        }
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == "iRacingSDK.DataSample")
                return typeof(iRacingSDK.DataSample);

            assemblyName = typeof(iRacingSDK.DataSample).Assembly.FullName;

            var typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }
    }
}
