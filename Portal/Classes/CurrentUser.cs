using Newtonsoft.Json;
using Portal.Models;

namespace Portal.Classes
{
    public class CurrentUser
    {
        public string? Login { get; set; }
        public string? MarketID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
        public Role? Roles { get; set; }

        public CurrentUser()
        {

        }

        public void SetValueCookie(HttpResponse response, string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddDays(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddDays(10);

            response.Cookies.Append(key, value, option);
        }

        public void SetValueCookie(HttpResponse response, string key, CurrentUser value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddDays(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddDays(10);

            response.Cookies.Append(key, JsonConvert.SerializeObject(value), option);
        }

        public string GetValueCookie(HttpRequest request, string key)
        {
            //read cookie from IHttpContextAccessor  
            //string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];

            //read cookie from Request object  
            string cookieValueFromReq = request.Cookies[key];

            return cookieValueFromReq;
        }

        //public CurrentUser GetValueCookie(HttpRequest request, string key)
        //{
        //    //read cookie from IHttpContextAccessor  
        //    //string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];

        //    //read cookie from Request object  
        //    CurrentUser value = JsonConvert.DeserializeObject<CurrentUser>(request.Cookies[key]);//request.Cookies[key];

        //    //return cookieValueFromReq;
        //    return value;
        //}


        public void Remove(HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
        }
    }
}
