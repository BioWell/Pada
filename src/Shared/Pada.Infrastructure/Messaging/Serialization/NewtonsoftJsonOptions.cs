using System.Collections.Generic;
using Newtonsoft.Json;
using Pada.Abstractions.Collections;
using Pada.Infrastructure.Collections;

namespace Pada.Infrastructure.Messaging.Serialization
{
    public class NewtonsoftJsonOptions
    {
        public IList<JsonConverter> Converters { get; set; }
        public ITypeList UnSupportedTypes { get; }

        public NewtonsoftJsonOptions()
        {
            Converters = new List<JsonConverter>();
            UnSupportedTypes = new TypeList();
        }
    }
}