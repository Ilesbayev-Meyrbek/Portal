using Newtonsoft.Json;
using Portal.DB;
using Portal.DTO;
using Portal.Models;
using System.Data.Entity;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Portal.Controllers
{
    [AllowAnonymous]
    //[Authorize]
    public class PortalController : ControllerBase
    {
        private ScaleContext scaleContext;
        private DataContext dataContext;

        public PortalController(DataContext dataContext, ScaleContext scaleContext)
        {
            this.scaleContext = scaleContext;
            this.dataContext = dataContext;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            this.HttpContext.Session.Set("access_token", Encoding.ASCII.GetBytes(token));
            return Ok(token);
        }
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<OutputMarket>> GetMarkets()
        {//Portal/GetMarkets
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            var markets = dataContext.Markets.ToList();
            var categories = dataContext.Categories.ToList();
            var scales = scaleContext.Scales.ToList();

            foreach (var market in markets)
            {
                var marketScales = scales.Where(x => x.MarketID == market.MarketID).ToList();
            }

            var outputMarkets = new List<OutputMarket>();
            var outputScales = new List<OutputScaleForMarket>();

            foreach (var scale in scales)
            {
                var categoryName = dataContext.MarketCategories.Where(x => x.MarketId == scale.MarketID && x.CategoryOrder == scale.DefaultCategoryIndex).Select(x => x.Category.RuName).FirstOrDefault();

                outputScales.Add(new OutputScaleForMarket
                {
                    Id = scale.ID,
                    MarketID = scale.MarketID,
                    Number = scale.ScaleName,
                    CategoryName = categoryName ?? "По умолчанию",
                    IP = scale.IP,
                    Status = false,
                    Type = scale.ScaleType
                });
            }
            foreach (var market in markets)
            {
                outputMarkets.Add(new OutputMarket
                {
                    Id = market.MarketID,
                    Name = market.Name,
                    Address = market.Address,
                    Scales = outputScales.Where(x => x.MarketID == market.MarketID).ToList(),
                });
            }

            return Ok(outputMarkets);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public async Task<IActionResult> SetDefaultCategory(int scaleId, int categoryIndex)
        {//Portal/SetDefaultCategory?scaleId=1&categoryIndex=1
            if (categoryIndex < 0 || categoryIndex > 4)
                return BadRequest();

            var scale = scaleContext.Scales.FirstOrDefault(x => x.ID.Equals(scaleId));

            if (scale != null)
            {
                scale.DefaultCategoryIndex = categoryIndex;
                scale.UpdateTime = DateTime.Now;
                scaleContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<OutputCategoryAndGroup>> GetCategoriesAndGroups()
        {
            var categories = dataContext.Categories.Select(x => new OutputCategoryAndGroup { Id = x.Id, Name = x.RuName }).ToList();

            foreach (var category in categories)
            {
                category.GroupsPLU = scaleContext.Groups.Where(x => x.CategoryId == category.Id).Select(x => x.SapID);
            }

            var groupWithoutCategories = scaleContext.Groups.Where(x => x.CategoryId == null).ToList();
            categories.Add(new OutputCategoryAndGroup { Id = categories.Last().Id + 1, Name = "Группы без категорий", GroupsPLU = groupWithoutCategories.Select(x => x.SapID) });

            return Ok(categories);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public async Task<IActionResult> SetGroupCategory(string groupPlu, int categoryId)
        {//api/Portal/SetGroupCategory?groupId=1&categoryId=1
            var group = scaleContext.Groups.FirstOrDefault(x => x.SapID == groupPlu);
            if (group is null)
                return NotFound();

            var category = dataContext.Categories.FirstOrDefault(x => x.Id == categoryId);
            if (category is null)
                return NotFound();

            category.UpdateTime = DateTime.Now;
            group.CategoryId = categoryId;

            var goods = scaleContext.ScalesGoods.Where(x => x.GroupId == group.Id).ToList();
            foreach (var good in goods)
                good.ReceiptGoodDate = DateTime.Now;

            scaleContext.SaveChanges();
            dataContext.SaveChanges();
            return Ok();
            //возвращать категорию
        }

        [HttpGet]
        public async Task<ActionResult<Category>> GetMarketCategories(string marketId)
        {
            var categories = dataContext.MarketCategories.Where(x => x.MarketId == marketId).OrderBy(x => x.CategoryOrder).Select(x => x.Category).ToList();
            
            return Ok(categories);
        }

        [HttpGet]
        public async Task<ActionResult<Category>> GetCategories()
        {
            var categories = dataContext.Categories.ToList();

            return Ok(categories);
        }

        [AcceptVerbs("GET", "PATCH")]
        [HttpPatch]
        public async Task<IActionResult> ChangeCategoriesOrder(string marketId, string categoryIdsOrder)
        {
            var categoriesOrder = dataContext.MarketCategories.Where(x => x.MarketId.Equals(marketId)).OrderByDescending(x => x.CategoryOrder).ToList();
            if (categoriesOrder.Count() != 0)
            {
                var listString = categoryIdsOrder.Split(',');
                var listInt = new List<int>();

                for (int i = 0; i < listString.Length; i++)
                    listInt.Add(int.Parse(listString[i]));

                for (int i = 0; i < listInt.Count(); i++)
                    foreach (var category in categoriesOrder)
                        if (category.CategoryId == listInt[i])
                        {
                            category.CategoryOrder = i;
                            category.UpdateTime = DateTime.Now;
                        }

                dataContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<OutputGood>> GetGoods(string marketId)
        {
            var list = new List<OutputGood>();
            {
                var goods = scaleContext.ScalesGoods
                .Where(x => x.MarketID.Equals(marketId))
                .Where(x=>x.GroupId != null)
                .Include(x=>x.Group)
                .Include(x => x.Image)
                .ToList();

                var goodIds = goods.Select(x => x.GroupId);//не работает Include
                var groups = scaleContext.Groups.Where(gr=> goodIds.Contains(gr.Id)).ToList();

                var imageIds = goods.Select(x => x.ImageId);//не работает Include
                var images = scaleContext.Images.Where(im => imageIds.Contains(im.Id)).ToList();

                foreach (var good in goods)
                {
                    var categoryName = dataContext.Categories.FirstOrDefault(x => x.Id == good.Group.CategoryId).RuName;
                    list.Add(new OutputGood
                    {
                        Id = good.ID,
                        Name = good.Name,
                        CategoryName = categoryName,
                        GroupPLU = good.Group.SapID,
                        Thumbnail = good.Image?.Thumbnail,
                        ImageId = good.ImageId,
                        PLU = good.PLU,
                        Code = good.Code,
                        Price = good.Price,
                    });
                }
            }

            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<OutputGood>> GetGoodsWithoutImg(string marketId)
        {
            var list = new List<OutputGood>();

            var goods = scaleContext.ScalesGoods
                .Where(x => x.MarketID.Equals(marketId))
                .Where(x => x.GroupId != null)
                .Where(x => x.ImageId == null)
                .Include(x=>x.Group)
                .ToList();

            var goodIds = goods.Select(x => x.GroupId);//не работает Include
            var groups = scaleContext.Groups.Where(gr => goodIds.Contains(gr.Id)).ToList();

            foreach (var good in goods)
            {
                var categoryName = dataContext.Categories.FirstOrDefault(x => x.Id == good.Group.CategoryId).RuName;
                list.Add(new OutputGood
                {
                    Id = good.ID,
                    Name = good.Name,
                    CategoryName = categoryName,
                    GroupPLU = good.Group.SapID,
                    PLU = good.PLU,
                    Code = good.Code,
                    Price = good.Price,
                });
            }

            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<OutputGroupAndCategory>> GetGroupCategories()
        {
            var list = new List<OutputGroupAndCategory>();
            var groups = scaleContext.Groups.ToList();
            var categories = dataContext.Categories.ToList();

            foreach (var group in groups)
            {
                list.Add(new OutputGroupAndCategory
                {
                    Id = group.Id,
                    GroupSAP = group.SapID,
                    CategoryRuName = categories.FirstOrDefault(x => x.Id.Equals(group.CategoryId))?.RuName ?? "Нет категории"
                });
            }

            return Ok(list);
        }

        [HttpGet]
        public JsonResult GetRole(string login)
        {
            var admin = dataContext.Admins.FirstOrDefault(x => x.Login == login);
            if (admin != null)
                return new JsonResult(new OutputMarketWithRole { Role = "admin" });

            var user = dataContext.Users.FirstOrDefault(x => x.Login == login);
            if (user != null)
            {
                var scales = scaleContext.Scales.ToList();

                var categories = dataContext.Categories.ToList();
                var market = dataContext.Markets.FirstOrDefault(x => x.MarketID.Equals(user.MarketID));

                var marketScales = scales.Where(x => x.MarketID == market.MarketID).ToList();

                var outputScales = new List<OutputScaleForMarket>();

                foreach (var scale in scales)
                {
                    var categoryName = dataContext.MarketCategories.Where(x => x.MarketId == scale.MarketID && x.CategoryOrder == scale.DefaultCategoryIndex).Select(x => x.Category.RuName).FirstOrDefault();

                    outputScales.Add(new OutputScaleForMarket
                    {
                        Id = scale.ID,
                        MarketID = scale.MarketID,
                        Number = scale.ScaleName,
                        CategoryName = categoryName ?? "По умолчанию",
                        IP = scale.IP,
                        Status = false,
                        Type = scale.ScaleType
                    });
                }
                var outputMarkets = new OutputMarketWithRole
                {
                    Id = market.MarketID,
                    Role = "user",
                    Name = market.Name,
                    Address = market.Address,
                    Scales = outputScales.Where(x => x.MarketID == market.MarketID).ToList(),
                };

                return new JsonResult(outputMarkets);
            }
            return null;
        }
        
        private string ConvertImgToBase64(string path)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public async Task<IActionResult> SetGoodsImage(string goodIds)
        {
            var file = HttpContext.Request.Form.Files.Count > 0 ?
            HttpContext.Request.Form.Files[0] : null;

            if (file == null)
                return BadRequest();

            var listGoodIds = goodIds.Split(',').Select(x => long.Parse(x)).ToList();
            var goods = (scaleContext.ScalesGoods.Where(x => listGoodIds.Contains(x.ID))).ToList();

            string bDate;
            string bThumb;
            System.Drawing.Image thumbnail;
            System.Drawing.Image sourceimage;

            try
            {
                sourceimage = System.Drawing.Image.FromStream(file.OpenReadStream());

                Image temp = new Image();
                using (var ms = new MemoryStream())
                {
                    sourceimage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bDate = Convert.ToBase64String(ms.ToArray());
                }

                temp.Data = bDate;
                thumbnail = sourceimage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

                using (var ms = new MemoryStream())
                {
                    thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bThumb = Convert.ToBase64String(ms.ToArray());
                }
                temp.Thumbnail = bThumb;
                temp.UpdateTime = DateTime.Now;
                var newImage = scaleContext.Images.Add(temp);
                await scaleContext.SaveChangesAsync();

                foreach (var good in goods)
                {
                    var goodInDB = scaleContext.ScalesGoods.FirstOrDefault(x => x.ID == good.ID);
                    goodInDB.Image = temp;
                    goodInDB.ImageId = temp.Id;
                    goodInDB.ReceiptGoodDate = DateTime.Now;
                }
                await scaleContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<Token> GetElibilityToken()
        {
            //https://localhost:7158/Portal/GetElibilityToken

            var client = new HttpClient();
            string baseAddress = @"https://graph.microsoft.com/v1.10/token";

            string grant_type = "client_credentials";
            string client_id = "16330d2e-f85f-4d88-b595-e251c7e2683d";
            string client_secret = "PY_8Q~3Btq1Fgmtu.4jMHz1YblQ1VDop7F9YEdee";

            var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                };

            HttpResponseMessage tokenResponse = await client.PostAsync(baseAddress, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            Token tok = JsonConvert.DeserializeObject<Token>(jsonContent);
            return tok;
        }


        public class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

    }
}
