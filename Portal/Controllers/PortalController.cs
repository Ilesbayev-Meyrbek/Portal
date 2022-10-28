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
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using Portal.Services.Interfaces;

namespace Portal.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class PortalController : ControllerBase
    {
        private ScaleContext scaleContext;
        private DataContext dataContext;
        private IMarketService marketService;
        private ICategoryService categoryService;
        private IScaleService scaleService;
        private IGroupService groupService;
        private IMarketCategoryService marketCategoryService;
        private IScalesGoodService scalesGoodService;

        public PortalController(DataContext dataContext, ScaleContext scaleContext, 
            IMarketService marketService, 
            ICategoryService categoryService, 
            IScaleService scaleService, 
            IGroupService groupService,
            IMarketCategoryService marketCategoryService,
            IScalesGoodService scalesGoodService)
        {
            this.scaleContext = scaleContext;
            this.dataContext = dataContext;
            this.marketService = marketService;
            this.categoryService = categoryService;
            this.scaleService = scaleService;
            this.groupService = groupService;
            this.marketCategoryService = marketCategoryService;
            this.scalesGoodService = scalesGoodService;
        }

        [HttpGet]
        public async Task<ActionResult<OutputMarket>> GetMarkets()
        {//Portal/GetMarkets
            
            var markets = (await marketService.GetAllAsync()).Data;
            var categories = (await categoryService.GetAllAsync()).Data;
            var scales = (await scaleService.GetAllAsync()).Data;

            foreach (var market in markets)
            {
                var marketScales = scales.Where(x => x.MarketID == market.MarketID).ToList();
            }

            var outputMarkets = new List<OutputMarket>();
            var outputScales = new List<OutputScaleForMarket>();

            foreach (var scale in scales)
            {
                var categoryName = (await marketCategoryService.GetForMarketAsync(scale.MarketID)).Data.Where(x=> x.CategoryOrder == scale.DefaultCategoryIndex).Select(x => x.Category.RuName).FirstOrDefault();

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

            var scale = (await scaleService.GetAsync(scaleId)).Data;

            if (scale is null)
                return NotFound();

            await scaleService.ChangeCategory(scale, categoryIndex);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<OutputCategoryAndGroup>> GetCategoriesAndGroups()
        {
            var categories = (await categoryService.GetAllAsync()).Data;
            var groups = (await groupService.GetAllAsync()).Data;

            if (categories.Count == 0 || groups.Count == 0)
                return NotFound();

            var outputCategories = categories.Select(x => new OutputCategoryAndGroup { Id = x.Id, Name = x.RuName }).ToList();

            foreach (var category in outputCategories)
            {
                category.GroupsPLU = groups.Where(x => x.CategoryId == category.Id).Select(x => x.SapID);
            }

            var groupWithoutCategories = groups.Where(x => x.CategoryId == null).ToList();
            outputCategories.Add(new OutputCategoryAndGroup { Id = outputCategories.Last().Id + 1, Name = "Группы без категорий", GroupsPLU = groupWithoutCategories.Select(x => x.SapID) });

            return Ok(outputCategories);
        }

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        public async Task<IActionResult> SetGroupCategory(string groupPlu, int categoryId)
        {//api/Portal/SetGroupCategory?groupPlu=1&categoryId=1
            var group = (await groupService.GetAsync(groupPlu)).Data;
            if (group is null)
                return NotFound();

            var category = (await categoryService.GetAsync(categoryId)).Data;
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
            var categories = (await marketCategoryService.GetForMarketAsync(marketId)).Data
                .OrderBy(x => x.CategoryOrder)
                .Select(x => x.Category)
                .ToList();

            var output = categories.Select(x => new Category
            {
                Id = x.Id,
                RuName = x.RuName,
                EnName = x.EnName,
                UzName = x.UzName,
            });
            return Ok(output);
        }

        [HttpGet]
        public async Task<ActionResult<Category>> GetCategories()
        {
            var categories = (await categoryService.GetAllAsync()).Data;

            return Ok(categories);
        }

        [AcceptVerbs("GET", "PATCH")]
        [HttpPatch]
        public async Task<IActionResult> ChangeCategoriesOrder(string marketId, string categoryIdsOrder)
        {
            var categoriesOrder = (await marketCategoryService.GetForMarketAsync(marketId)).Data.OrderByDescending(x => x.CategoryOrder).ToList();

            if (categoriesOrder.Count() == 0)
                return NotFound();

            await marketCategoryService.ChangeCategoriesOrderAsync(categoriesOrder, categoryIdsOrder);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<OutputGood>> GetGoods(string marketId)
        {
            var list = new List<OutputGood>();
            {
                var goods = (await scalesGoodService.GetGoodsAsync(marketId)).Data;

                var imageIds = goods.Select(x => x.ImageId);//не работает Include
                var images = scaleContext.Images.Where(im => imageIds.Contains(im.Id)).ToList();

                foreach (var good in goods)
                {
                    var categoryName = (await categoryService.GetAsync(good.Group.CategoryId??-1)).Data.RuName;
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

            var goods = (await scalesGoodService.GetGoodsWithoutImgAsync(marketId)).Data;

            foreach (var good in goods)
            {
                var categoryName = (await categoryService.GetAsync(good.Group.CategoryId ?? -1)).Data.RuName;
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
            var groups = (await groupService.GetAllAsync()).Data;
            var categories = (await categoryService.GetAllAsync()).Data;

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

            var result = await scalesGoodService.SetGoodImage(file, goodIds);

            if (result.Success)
                return BadRequest();
            return Ok();
        }
    }
}
