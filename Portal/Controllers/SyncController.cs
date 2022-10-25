
using Newtonsoft.Json;
using Portal.DB;
using Portal.DTO;
using Portal.Models;
using System.Data.Entity;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Portal.Controllers
{
    [AllowAnonymous]
    public class SyncController : ControllerBase
    {
        private ScaleContext scaleContext;
        private DataContext dataContext;

        public SyncController(DataContext dataContext, ScaleContext scaleContext)
        {
            this.scaleContext = scaleContext;
            this.dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<bool>> isUpdated(string IP)
        {//https://localhost:7158/Sync/isUpdated?IP=172.16.80.6

            var scale = GetScale(IP);
            if (scale is null)
                return NotFound();

            var scaleLastSyncTime = scale.ScalesSyncStatus.LastSyncTime;
            var goodsUpdateTime = scaleContext.ScalesGoods.Where(x => x.MarketID == scale.MarketID).Select(x => x.ReceiptGoodDate).ToList();
            var categoriesUpdateTime = dataContext.Categories.Select(x => x.UpdateTime).ToList();
            var categoriesScaleUpdateTime = dataContext.MarketCategories.Where(x => x.MarketId == scale.MarketID).Select(x => x.UpdateTime).ToList();

            if (scaleLastSyncTime < scale.UpdateTime) return Ok(false);

            foreach (var categoryUT in categoriesUpdateTime)
                if (scaleLastSyncTime < categoryUT) return Ok(false);

            foreach (var goodUT in goodsUpdateTime)
                if (scaleLastSyncTime < goodUT) return Ok(false);

            foreach (var goodUT in categoriesScaleUpdateTime)
                if (scaleLastSyncTime < goodUT) return Ok(false);

            return Ok(true);
        }

        [AcceptVerbs("GET", "PATCH")]
        [HttpPatch]
        public async Task<ActionResult> UpdateLastSyncTime(string IP, DateTime startUpdateTime)
        {
            var scaleLastSyncTime = GetScale(IP);
            scaleLastSyncTime.ScalesSyncStatus.LastSyncTime = startUpdateTime;

            scaleContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        //https://localhost:7158/Sync/GetUpdatedGoods?IP=172.16.80.6
        public async Task<ActionResult<GoodsForScale>> GetUpdatedGoods(string IP)
        {
            var scale = GetScale(IP);
            var listCategoriesId = GetCategoriesId(IP);

            var updatedGoods = scaleContext.ScalesGoods.Include(x => x.Group)
                .Where(x=>x.MarketID == scale.MarketID)
                .Where(x => x.ReceiptGoodDate > scale.ScalesSyncStatus.LastSyncTime)
                .Where(x => listCategoriesId.Contains(x.Group.CategoryId ?? -1))
                .ToList();

            var goodIds = updatedGoods.Select(x => x.GroupId);//не работает Include
            var groups = scaleContext.Groups.Where(gr => goodIds.Contains(gr.Id)).ToList();

            var dto = updatedGoods.Select(x => new GoodsForScale
            {
                ID = x.ID,
                CategoryId = x.Group.CategoryId,
                SAPID = x.SAPID,
                Name = x.Name,
                PLU = x.PLU,
                Price = x.Price,
                GroupId = x.GroupId,
                ImageId = x.ImageId,
                Weight = x.Weight,
                Code = x.Code,
            }).ToList();

            return Ok(dto);
        }
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetUpdatedCategory(string IP)
        {
            var scaleLastSyncTime = GetScale(IP);
            var listCategoriesId = GetCategoriesId(IP);

            var updatedCategory = dataContext.Categories
                .Where(x => x.UpdateTime > scaleLastSyncTime.ScalesSyncStatus.LastSyncTime)
                .Where(x => listCategoriesId.Contains(x.Id)).ToList();

            return Ok(updatedCategory);
        }
        [HttpGet]
        public async Task<ActionResult<List<CategoriesForScale>>> GetCategoriesScale(string IP)
        {
            var scale = scaleContext.Scales.FirstOrDefault(x => x.IP == IP);
            var market = dataContext.Markets.FirstOrDefault(x => x.MarketID == scale.MarketID);
            var list = dataContext.MarketCategories.Where(x => x.MarketId == market.MarketID)
                .Select(x => new CategoriesForScale { CategoryId = x.CategoryId, CategoryRuName = x.Category.RuName, CategoryUzName = x.Category.UzName, CategoryEnName = x.Category.EnName, CategoryOrder = x.CategoryOrder, isDefault = false })
                .ToList();

            foreach (var category in list)
                if (category.CategoryOrder == scale.DefaultCategoryIndex)
                    category.isDefault = true;

            return Ok(list);
        }

        //[HttpGet]
        //public JsonResult<Settings> GetUpdatedSettings(string IP)
        //{
        //    var scale = scaleContext.Scales.Include("ScalesSyncStatus").FirstOrDefault(x => x.IP == IP);
        //    var settings = scaleContext.Settings.FirstOrDefault(x => x.Scale_ID == scale.ID && x.UpdateTime > scale.ScalesSyncStatus.LastSyncTime);
        //    var output = new JsonResult<Settings>(settings, new JsonSerializerSettings(), Encoding.UTF8, this);
        //    return output;
        //}

        [HttpGet]
        public async Task<ActionResult<List<long>>> GetGoodsId(string IP)
        {
            var scale = scaleContext.Scales.FirstOrDefault(x => x.IP == IP);
            var listCategoriesId = GetCategoriesId(IP);

            var goodsId = scaleContext.ScalesGoods
                .Where(x => x.MarketID == scale.MarketID)
                .Where(x => listCategoriesId.Contains(x.Group.CategoryId ?? -1))
                .Select(x => x.ID).ToList();

            return Ok(goodsId);
        }
        [HttpGet]
        public async Task<ActionResult<List<ImageForScale>>> GetUpdatedImages(string IP)
        {
            var scaleLastSyncTime = GetScale(IP);
            var listCategoriesId = GetCategoriesId(IP);
            var listGoodId = scaleContext.ScalesGoods.Where(x => listCategoriesId.Contains(x.Group.CategoryId ?? -1)).Select(x => x.ImageId).ToList();
            var updatedImages = scaleContext.Images
                .Where(x => x.UpdateTime > scaleLastSyncTime.ScalesSyncStatus.LastSyncTime)
                .Where(x => listGoodId.Contains(x.Id))
                .Select(x => new ImageForScale { Id = x.Id, Data = x.Data })
                .ToList();

            return Ok(updatedImages);
        }

        [HttpGet]
        public async Task<ActionResult<List<int>>> GetImagesId()
        {
            var imagesId = scaleContext.Images.Select(x => x.Id).ToList();

            return Ok(imagesId);
        }

        private List<int> GetCategoriesId(string IP)
        {
            var scale = scaleContext.Scales.FirstOrDefault(x => x.IP == IP);
            var market = dataContext.Markets.FirstOrDefault(x => x.MarketID == scale.MarketID);
            var listCategoriesId = dataContext.MarketCategories.Where(x => x.MarketId == market.MarketID).Select(x => x.CategoryId).ToList();
            return listCategoriesId;
        }
        private Scale GetScale(string IP)
        {
            var scale = scaleContext.Scales.FirstOrDefault(x => x.IP == IP);
            if (scale is null)
                return null;

            var scaleSyncStatus = scaleContext.ScalesSyncStatuses.FirstOrDefault(x => x.Scale_ID.Equals(scale.ID));
            if(scaleSyncStatus is null)
                return null;

            return scale;
        }
    }
}