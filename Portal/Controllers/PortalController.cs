using Newtonsoft.Json;
using Portal.DB;
using Portal.DTO;
using Portal.Models;
using System.Data.Entity;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portal.Controllers
{
    [Authorize]
    public class PortalController : ControllerBase
    {
        private ScaleContext scaleContext;
        private DataContext dataContext;

        public PortalController(DataContext dataContext, ScaleContext scaleContext)
        {
            this.scaleContext = scaleContext;
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<OutputMarket>> GetMarkets()
        {//Portal/GetMarkets

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

        [HttpPost]
        public async Task<IActionResult> SetDefaultCategory(int scaleId, int categoryIndex)
        {//api/Portal/SetDefaultCategory?scaleId=1&categoryId=1
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
        public JsonResult GetCategoriesAndGroups()
        {
            var categories = dataContext.Categories.Select(x => new OutputCategoryAndGroup { Id = x.Id, Name = x.RuName }).ToList();

            foreach (var category in categories)
            {
                category.GroupsPLU = scaleContext.Groups.Where(x => x.CategoryId == category.Id).Select(x => x.Name);
            }

            var groupWithoutCategories = scaleContext.Groups.Where(x => x.CategoryId == null).ToList();
            categories.Add(new OutputCategoryAndGroup { Id = categories.Last().Id + 1, Name = "Группы без категорий", GroupsPLU = groupWithoutCategories.Select(x => x.Name) });

            var output = new JsonResult(categories);

            return output;
        }

        [HttpPost]
        public async Task<IActionResult> SetGroupCategory(int groupPlu, int categoryId)
        {//api/Portal/SetGroupCategory?groupId=1&categoryId=1
            var group = scaleContext.Groups.FirstOrDefault(x => x.Name == groupPlu);
            if (group is null)
                return NotFound();

            var category = dataContext.Categories.FirstOrDefault(x => x.Id == categoryId);
            if (category is null)
                return NotFound();

            category.UpdateTime = DateTime.Now;

            var goods = scaleContext.ScalesGoods.Where(x => x.GroupId == group.Id).ToList();
            foreach (var good in goods)
                good.ReceiptGoodDate = DateTime.Now;

            group.Category = category;

            scaleContext.SaveChanges();
            return Ok();
            //возвращать категорию
        }

        [HttpGet]
        public JsonResult GetMarketCategories(string marketId)
        {
            var categories = dataContext.MarketCategories.Include("Category").Where(x => x.MarketId == marketId).OrderBy(x => x.CategoryOrder).Select(x => x.Category).ToList();
            var cat = categories.ToList();
            var output = new JsonResult(cat);

            return output;
        }

        [HttpGet]
        public JsonResult GetCategories()
        {
            var categories = dataContext.Categories.ToList();
            var output = new JsonResult(categories);

            return output;
        }

        [HttpPatch]
        public async Task<IActionResult> ChangeCategoriesOrder(string marketId, string categoryIdsOrder)
        {
            var categoriesOrder = dataContext.MarketCategories.Where(x => x.MarketId.Equals(marketId)).OrderByDescending(x => x.CategoryOrder);
            if (categoriesOrder != null)
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

                scaleContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        public JsonResult GetGoods(string marketId)
        {
            var list = new List<OutputGood>();
            {
                var goods = scaleContext.ScalesGoods
                .Where(x => x.MarketID.Equals(marketId))
                .Include("Group")
                .Include("Image")
                .ToList();

                foreach (var good in goods)
                {
                    var categoryName = dataContext.Categories.FirstOrDefault(x => x.Id == good.Group.CategoryId).RuName;
                    list.Add(new OutputGood
                    {
                        Id = good.ID,
                        Name = good.Name,
                        CategoryName = categoryName,
                        GroupPLU = good.Group.Name,
                        Thumbnail = good.Image?.Thumbnail,
                        ImageId = good.ImageId,
                        PLU = good.PLU,
                        Code = good.Code,
                        Price = good.Price,
                    });
                }
            }

            var output = new JsonResult(list);

            return output;
        }

        [HttpGet]
        public JsonResult GetGoodsWithoutImg(string marketId)
        {
            var list = new List<OutputGood>();

            var goods = scaleContext.ScalesGoods
                .Where(x => x.MarketID.Equals(marketId))
                .Where(x => x.ImageId == null)
                .Include("Group")
                .ToList();

            foreach (var good in goods)
            {
                var categoryName = dataContext.Categories.FirstOrDefault(x => x.Id == good.Group.CategoryId).RuName;
                list.Add(new OutputGood
                {
                    Id = good.ID,
                    Name = good.Name,
                    CategoryName = categoryName,
                    GroupPLU = good.Group.Name,
                    PLU = good.PLU,
                    Code = good.Code,
                    Price = good.Price,
                });
            }

            var output = new JsonResult(list);

            return output;
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

        //[HttpPost]
        //public async Task<IActionResult> SetGoodsImage(string goodIds)
        //{
        //    var file = HttpContext.Request.Files.Count > 0 ?
        //    HttpContext.Request.Files[0] : null;

        //    if (file == null)
        //        return BadRequest();

        //    var listGoodIds = goodIds.Split(',').Select(x => long.Parse(x)).ToList();
        //    var goods = (scaleContext.ScalesGoods.Where(x => listGoodIds.Contains(x.ID))).ToList();

        //    string bDate;
        //    string bThumb;
        //    System.Drawing.Image thumbnail;
        //    System.Drawing.Image sourceimage;

        //    try
        //    {
        //        sourceimage = System.Drawing.Image.FromStream(file.InputStream);

        //        Image temp = new Image();
        //        using (var ms = new MemoryStream())
        //        {
        //            sourceimage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //            bDate = Convert.ToBase64String(ms.ToArray());
        //        }

        //        temp.Data = bDate;
        //        thumbnail = sourceimage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

        //        using (var ms = new MemoryStream())
        //        {
        //            thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //            bThumb = Convert.ToBase64String(ms.ToArray());
        //        }
        //        temp.Thumbnail = bThumb;
        //        temp.UpdateTime = DateTime.Now;
        //        var newImage = scaleContext.Images.Add(temp);
        //        await scaleContext.SaveChangesAsync();

        //        foreach (var good in goods)
        //        {
        //            var goodInDB = await scaleContext.ScalesGoods.FirstOrDefaultAsync(x => x.ID == good.ID);
        //            goodInDB.Image = newImage;
        //            goodInDB.ImageId = newImage.Id;
        //            goodInDB.ReceiptGoodDate = DateTime.Now;
        //        }
        //        await scaleContext.SaveChangesAsync();

        //        return Ok();
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        //    #region Not needed
        //    [HttpGet]
        //    public JsonResult<List<OutputGood>> GetGoods(string marketId)
        //    {
        //        var list = dataContext.ScalesGoods
        //            .Include("Image")
        //            .Where(x => x.MarketID == marketId)
        //            .Select(x => new OutputGood { Id = x.ID, Name = x.Name, PLU = x.PLU, Price = x.Price, CategoryName = x.Group.Category.RuName, GroupPLU = x.Group.Name, ImageId = x.ImageId, Image = x.Image }).ToList();

        //        var output = new JsonResult<List<OutputGood>>(list, new JsonSerializerSettings(), Encoding.UTF8, this);

        //        return output;
        //    }


        //    [HttpPost]
        //    public IHttpActionResult UpdateImage(int id)
        //    {
        //        var file = HttpContext.Current.Request.Files.Count > 0 ?
        //             HttpContext.Current.Request.Files[0] : null;
        //        System.Drawing.Image sourceimage = System.Drawing.Image.FromStream(file.InputStream);
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            string bDate;
        //            string bThumb;
        //            System.Drawing.Image thumbnail;
        //            var temp = dataContext.Images.Where(x => x.Id == id).FirstOrDefault();
        //            if (temp == null)
        //                return BadRequest();

        //            using (var ms = new MemoryStream())
        //            {
        //                sourceimage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //                bDate = Convert.ToBase64String(ms.ToArray());
        //            }

        //            temp.Data = bDate;
        //            thumbnail = sourceimage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);


        //            using (var ms = new MemoryStream())
        //            {
        //                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //                bThumb = Convert.ToBase64String(ms.ToArray());
        //            }
        //            temp.Thumbnail = bThumb;
        //            temp.UpdateTime = DateTime.Now;
        //            dataContext.Entry(temp).State = System.Data.Entity.EntityState.Modified;
        //            dataContext.SaveChanges();
        //            return Ok();
        //        }
        //        else
        //            return BadRequest();
        //    }

        //[HttpPost]
        //public IHttpActionResult AddImage()
        //{
        //    var file = HttpContext.Current.Request.Files.Count > 0 ?
        //         HttpContext.Current.Request.Files[0] : null;
        //    string bDate;
        //    string bThumb;
        //    System.Drawing.Image thumbnail;

        //    if (file != null && file.ContentLength > 0)
        //    {
        //        try
        //        {
        //            System.Drawing.Image sourceimage = System.Drawing.Image.FromStream(file.InputStream);
        //            Image temp = new Image();
        //            using (var ms = new MemoryStream())
        //            {
        //                sourceimage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //                bDate = Convert.ToBase64String(ms.ToArray());
        //            }

        //            temp.Data = bDate;
        //            thumbnail = sourceimage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

        //            using (var ms = new MemoryStream())
        //            {
        //                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //                bThumb = Convert.ToBase64String(ms.ToArray());
        //            }
        //            temp.Thumbnail = bThumb;
        //            temp.UpdateTime = DateTime.Now;
        //            dataContext.Images.Add(temp);
        //            dataContext.SaveChanges();
        //        }
        //        catch (Exception)
        //        {
        //            return BadRequest();
        //        }
        //        return Ok();
        //    }
        //    return BadRequest();
        //}



        //    [Route("{id:int")]
        //    [HttpGet]
        //    public string GetImage(int id)
        //    {//api/Portal/AddImage
        //        var image = dataContext.Images.Where(x => x.Id == id).FirstOrDefault();

        //        if (image == null)
        //            return String.Empty;


        //        return image.Data;

        //    }

        //    [Route("{id:int")]
        //    [HttpGet]
        //    public string GetThumbnail(int id)
        //    {
        //        var image = dataContext.Images.Where(x => x.Id == id).FirstOrDefault();

        //        if (image == null)
        //            return String.Empty;


        //        return image.Thumbnail;
        //    }

        //        return image.Thumbnail;
        //    }

        //    private int GetCategoryId(string categories, int index)
        //    {
        //        if (categories is null)
        //            return -1;
        //        var splitCategories = categories.Split(',');
        //        if (index < 0 || index >= categories.Length)
        //        {
        //            throw new IndexOutOfRangeException();
        //        }
        //        bool parseResult = int.TryParse(splitCategories[index], out int id);
        //        if (!parseResult)
        //        {
        //            throw new InvalidCastException($"categories[{index}] содержало тип, отличный от Int32");
        //        }
        //        return id;
        //    }
        //    private byte[] ImgToByte64(string path)
        //    {
        //        using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
        //        {
        //            using (MemoryStream m = new MemoryStream())
        //            {
        //                image.Save(m, image.RawFormat);
        //                byte[] imageBytes = m.ToArray();

        //                // Convert byte[] to Base64 String
        //                string base64String = Convert.ToBase64String(imageBytes);
        //                return imageBytes;
        //            }
        //        }
        //    }

        //    private System.Drawing.Image Byte64ToImg(byte[] img)
        //    {

        //        System.Drawing.Image image;
        //        using (MemoryStream ms = new MemoryStream(img))
        //        {
        //            image = System.Drawing.Image.FromStream(ms);
        //        }

        //        return image;
        //    }
        //}

        //#endregion


    }
}
