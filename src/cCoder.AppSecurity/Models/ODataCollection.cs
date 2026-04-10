using Newtonsoft.Json;

namespace cCoder.AppSecurity.Models;

public class ODataCollection<TCollectionType>
{
    [JsonProperty("@odata.context")]
    public string ODataContext { get; set; }

    public IEnumerable<TCollectionType> Value { get; set; }
}
