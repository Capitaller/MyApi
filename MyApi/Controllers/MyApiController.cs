using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.IO;
//using MyApi.Models;
//using MyApi.Models.Entities;


namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class MyApiController : ControllerBase
    {
        private const string path = @"UserL.json";
        [HttpGet]
        [Route("money/{id}")]
        public async Task<ObjectResult> mon(string id)
        {
            Regex regex = new Regex(@"[A-Z]{3}");
           
            if ((regex.IsMatch(id) && id.Length == 3) == false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
           
            var client = new HttpClient();
            var content = await client.GetStringAsync("https://currencyapi.net/api/v1/rates?key=8d4b1e2ef6c0235840439e0337e6972ace1c&base=USD");

            Money currency = JsonConvert.DeserializeObject<Money>(content);
            decimal price = currency.Rates[id];

            return new ObjectResult(price);

        }
        [HttpGet]
        [Route("money/list")]
        public async Task<ObjectResult> list()
        {
            var client = new HttpClient();
            var content = await client.GetStringAsync("https://currencyapi.net/api/v1/rates?key=8d4b1e2ef6c0235840439e0337e6972ace1c&base=USD");

            Money currency = JsonConvert.DeserializeObject<Money>(content);
            List<string> curr = new List<string>();
            foreach (KeyValuePair<string, decimal> keyValue in currency.Rates)
            {
                curr.Add(keyValue.Key);
            }

            return new ObjectResult(curr);
        }
        [HttpGet]
        [Route("money/forecast/{Cur}")]
        public async Task<ObjectResult> forecast(string Cur)
        {
            Regex regex = new Regex(@"[A-Z]{3}");
           
            if ((regex.IsMatch(Cur) && Cur.Length ==3 )==false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            string forecast;
            if (Cur == "USD")
            {
                return new ObjectResult("I don't know, ask Trump");
            }
            var client = new HttpClient();
            
            
          
            decimal[] prises = new decimal[23];
            decimal currentPrice;
             var content = await client.GetStringAsync("https://fcsapi.com/api-v2/forex/history?symbol=USD/"+Cur+"&period=1d&from=2020-03-01&access_key=NXVppu3eBdZj3pWoqC6bCqavML8bltAlWgDv1j0a1BZYzIuRTL");

             Forex currency = JsonConvert.DeserializeObject<Forex>(content);
            string x_string;
          
            int length = currency.Response.Length -1;
           

            for (int i=0; i<23; i++ )
            {
                length--;
                x_string = currency.Response[length].C;
                x_string = x_string.Replace(".", ",");
                currentPrice = Convert.ToDecimal(x_string);
                prises[i] = currentPrice ;
            }
            decimal maxCUR = prises.Max<decimal>();
            decimal Point = Convert.ToDecimal(1.15);
            
            decimal minCUR = prises.Min<decimal>();
            decimal FirstSeven = prises[0];
            decimal SecondSeven= prises[7];
            decimal ThirdSeven= prises[14];
            decimal averageCur = prises.Average();
            for (int i=1;i<7; i++)
            {
                FirstSeven += prises[i];
                SecondSeven += prises[7+i];
                ThirdSeven += prises[14+i];
            }
            
           if(FirstSeven < SecondSeven&& SecondSeven < ThirdSeven)
            {
                if ((averageCur*Point > maxCUR) && (averageCur/Point < minCUR))
                {
                    forecast = "Down";
                }
                else
                {
                    forecast = "Up";
                }
            }
            else
            {
                forecast = "Up";
            }

            return new ObjectResult(forecast);



        }
        [HttpGet]
        [Route("money/from{id1}/to{id2}")]
        public async Task<ObjectResult> Converter(string id1, string id2)
        {
            Regex regex = new Regex(@"[A-Z]{3}");

            if ((regex.IsMatch(id1)&& regex.IsMatch(id2) && id1.Length == 3 && id2.Length == 3) == false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            var client = new HttpClient();
            var content = await client.GetStringAsync("https://currencyapi.net/api/v1/rates?key=8d4b1e2ef6c0235840439e0337e6972ace1c&base=USD");

            Money currency = JsonConvert.DeserializeObject<Money>(content);
            decimal from = currency.Rates[id1];

            decimal to = currency.Rates[id2];

            decimal result = from / to;
            return new ObjectResult(result);

        }
        [HttpGet]
        [Route("money/AllUserCur/{id}")]
        public async Task<ObjectResult> AddCur(int id)
        {
            UserList dBUserList = JsonConvert.DeserializeObject<UserList>(System.IO.File.ReadAllText(path));
           
            string idCur;
            string idBase;
            Dictionary<string, decimal> AllUserCur = new Dictionary<string, decimal>();
            var client = new HttpClient();
            var content = await client.GetStringAsync("https://currencyapi.net/api/v1/rates?key=8d4b1e2ef6c0235840439e0337e6972ace1c&base=USD");

            Money currency = JsonConvert.DeserializeObject<Money>(content);
           
            for (int i = 0; i < dBUserList.users.Count; i++)
            {

                if (dBUserList.users[i].Id == id)
                {
                    for(int j=0; j< dBUserList.users[i].Currency.Count; j++)
                    {



                        idCur = dBUserList.users[i].Currency[j].Cur;
                        idBase = dBUserList.users[i].Base;
                        decimal from = currency.Rates[idBase];

                        decimal to = currency.Rates[idCur];

                        decimal result = from / to;
                      
                        result = decimal.Round(result, 3, MidpointRounding.ToEven);

                        AllUserCur.Add(idCur, result);
                    }
                }

            }
            return new ObjectResult(AllUserCur);
        }

        [HttpGet]
        [Route("money/calc/from/{num1}/{id1}/plus/{num2}/{id2}/to{id3}")]
        public async Task<ObjectResult> Calculator(string id1, string id2, decimal num1, decimal num2, string id3)
        {
            Regex regex = new Regex(@"[A-Z]{3}");

            if ((regex.IsMatch(id1) && regex.IsMatch(id2) && regex.IsMatch(id3) && id1.Length == 3 && id2.Length == 3 && id3.Length == 3) == false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            var client = new HttpClient();
            var content = await client.GetStringAsync("https://currencyapi.net/api/v1/rates?key=8d4b1e2ef6c0235840439e0337e6972ace1c&base=USD");

            Money currency = JsonConvert.DeserializeObject<Money>(content);
            List<string> curr = new List<string>();
            foreach (KeyValuePair<string, decimal> keyValue in currency.Rates)
            {
                curr.Add(keyValue.Key);
            }
            if ((curr.Contains(id1)) == false)
            {
               // Response.StatusCode = 415;

                return new ObjectResult(-1);
            }
            if ((curr.Contains(id2)) == false)
            {
               // Response.StatusCode = 415;

                return new ObjectResult(-1);
            }
            if ((curr.Contains(id3)) == false)
            {
               // Response.StatusCode = 415;

                return new ObjectResult(-1);
            }
            decimal to = currency.Rates[id3];

            decimal price1 = currency.Rates[id1];
            decimal price2 = currency.Rates[id2];
            decimal from1 = num1 * (1 / price1);
            decimal from2 = num2 * (1 / price2);

            decimal result1 = (from1 + from2) * to;
            result1 = decimal.Round(result1, 2, MidpointRounding.ToEven);
            return new ObjectResult(result1);

        }
        [HttpPost]
        [Route("money/AddUser")]
        public async Task<ObjectResult> AddUser([FromBody] UserId item1)
        {
           UserList dBUserList = JsonConvert.DeserializeObject<UserList>(System.IO.File.ReadAllText(path));
           if (dBUserList == null)
            {
                UserMoney p2 = new UserMoney
                {
                    Cur = "EUR"
                };
                List<UserMoney> users2 = new List<UserMoney>();
                users2.Add(p2);
                UserInfo p1 = new UserInfo
                {
                    Id = item1.Id,
                    Base = "UAH",
                    Currency = users2
                };
                List<UserInfo> users = new List<UserInfo>();
                users.Add(p1);
                UserList info = new UserList
                {
                    users = new List<UserInfo>(users)
                };

                await System.IO.File.WriteAllTextAsync(path, JsonConvert.SerializeObject(info, Formatting.Indented));
                dBUserList = JsonConvert.DeserializeObject<UserList>(System.IO.File.ReadAllText(path));
                return new ObjectResult(null);
            }
           
                    UserMoney p = new UserMoney
            {
                Cur = "USD"
            };
            List<UserMoney> users13 = new List<UserMoney>();
            users13.Add(p);
            UserInfo p12 = new UserInfo
            {
                Id = item1.Id,
                Base = "UAH",
                Currency = users13
            };
            for (int i = 0; i < dBUserList.users.Count; i++)
            {

                if (dBUserList.users[i].Id == item1.Id)
                {
                    return new ObjectResult(null);
                }
            }
            dBUserList.users.Add(p12);
            await System.IO.File.WriteAllTextAsync(path, JsonConvert.SerializeObject(dBUserList, Formatting.Indented));
            return new ObjectResult(null);
        }
        [HttpPut]
        [Route("money/AddCUr/{id}")]
        public async Task<ObjectResult> AddCur([FromBody] UserMoney item3, int id)
        {
            Regex regex = new Regex(@"[A-Z]{3}");

            if ((regex.IsMatch(item3.Cur) &&  item3.Cur.Length == 3) == false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            var client = new HttpClient();
            var content = await client.GetStringAsync("https://currencyapi.net/api/v1/rates?key=8d4b1e2ef6c0235840439e0337e6972ace1c&base=USD");

            Money currency = JsonConvert.DeserializeObject<Money>(content);
            List<string> curr = new List<string>();
            foreach (KeyValuePair<string, decimal> keyValue in currency.Rates)
            {
                curr.Add(keyValue.Key);
            }
            if ((curr.Contains(item3.Cur))==false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            UserList dBUserList = JsonConvert.DeserializeObject<UserList>(System.IO.File.ReadAllText(path));
            
            for (int i = 0; i < dBUserList.users.Count; i++)
            {

                if (dBUserList.users[i].Id == id)
                {
                   if(dBUserList.users[i].Currency.Count==5)
                    {
                        Response.StatusCode = 402;

                        return new ObjectResult(null);
                    }

                    UserMoney p = new UserMoney
                    {
                        Cur = item3.Cur
                    };
                    for (int j = 0; j < dBUserList.users[i].Currency.Count; j++)
                    {
                        if (dBUserList.users[i].Currency[j].Cur == item3.Cur)
                        {
                            Response.StatusCode = 409;
                         
                            return new ObjectResult(null);

                        }
                    }

                    dBUserList.users[i].Currency.Add(p);
                    break;
                }
            }
           await System.IO.File.WriteAllTextAsync(path, JsonConvert.SerializeObject(dBUserList, Formatting.Indented));
          
            return new ObjectResult(null);
        }
        [HttpPut]
        [Route("money/DeleteCUr/{id}")]
        public async Task<ObjectResult> DeleteCur([FromBody] UserMoney item3, int id)
        {

            UserList dBUserList = JsonConvert.DeserializeObject<UserList>(System.IO.File.ReadAllText(path));
            Regex regex = new Regex(@"[A-Z]{3}");

            if ((regex.IsMatch(item3.Cur) && item3.Cur.Length == 3) == false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            bool Success =false;
            for (int i = 0; i < dBUserList.users.Count; i++)
            {

                if (dBUserList.users[i].Id == id)
                {
                    if (dBUserList.users[i].Currency.Count == 1)
                    {
                        Response.StatusCode = 402;

                        return new ObjectResult(null);
                    }
                    UserMoney p = new UserMoney
                    {
                        Cur = item3.Cur
                    };
                    for (int j = 0; j < dBUserList.users[i].Currency.Count; j++)
                    {
                        
                        if (dBUserList.users[i].Currency[j].Cur == item3.Cur)
                        {

                            dBUserList.users[i].Currency.RemoveAt(j);
                            Success = true;

                        }
                       
                    }
                    
                }
            }
            if (Success ==false)
            {
                Response.StatusCode = 404;

                return new ObjectResult(null);
            }

            await System.IO.File.WriteAllTextAsync(path, JsonConvert.SerializeObject(dBUserList, Formatting.Indented));

            return new ObjectResult(null);
        }
        [HttpPut]
        [Route("money/AddNewBase/{id}")]
        public async Task<ObjectResult> AddNewBase([FromBody] UserInfo item3, int id)
        {

            UserList dBUserList = JsonConvert.DeserializeObject<UserList>(System.IO.File.ReadAllText(path));
            Regex regex = new Regex(@"[A-Z]{3}");

            if ((regex.IsMatch(item3.Base) && item3.Base.Length == 3) == false)
            {
                Response.StatusCode = 415;

                return new ObjectResult(null);
            }
            for (int i = 0; i < dBUserList.users.Count; i++)
            {
                if (dBUserList.users[i].Id == id)
                {

                    dBUserList.users[i].Base = item3.Base;
                    break;
                }
            }
            await System.IO.File.WriteAllTextAsync(path, JsonConvert.SerializeObject(dBUserList, Formatting.Indented));

            return new ObjectResult(null);
        }


    }
}