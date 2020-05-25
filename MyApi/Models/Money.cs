using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyApi.Controllers
{
    
    
        public class Money
        {
            [JsonProperty("valid")]
            public bool Valid { get; set; }

            [JsonProperty("base")]
            public string Base { get; set; }

            [JsonProperty("rates")]
            public Dictionary<string, decimal> Rates { get; set; }

        }
    public class UserId
    {
        public int Id { get; set; }
    }
    public class UserInfo
    {
        public int Id { get; set; }
        public string Base { get; set; }
        public List<UserMoney> Currency { get; set; }
    }
    public class UserList
    {
        public List<UserInfo> users { get; set; }
    }
    public class UserMoney
    {
        public string Cur { get; set; }
    }
    public partial class Forex
    {


        [JsonProperty("response")]
        public Response[] Response { get; set; }


    }



    public partial class Response
    {


        [JsonProperty("c")]
        public string C { get; set; }


    }

}
