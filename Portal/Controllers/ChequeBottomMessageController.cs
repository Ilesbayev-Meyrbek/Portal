using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Graph;
using Portal.DB;
using Portal.Models;
using Portal.Services;
using Portal.Services.Interfaces;
using System.Web.Http;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace Portal.Controllers
{
    [AllowAnonymous]
    public class ChequeBottomMessageController : Controller
    {
        private IChequeBottomMessageServices ChequeBottomMessageService;

        public ChequeBottomMessageController(IChequeBottomMessageServices services)
        {
            this.ChequeBottomMessageService = services;
        }

        public async Task<ActionResult> ChequeBottomMessages()
        {
            var ChequeBottomMessages= await ChequeBottomMessageService.GetAllAsync();


            return View(ChequeBottomMessages.Data);
        }

        public async Task<ActionResult> CreateChequeBottomMessage() 
            => View(new ChequeBottomMessage());

        [HttpPost]
        public async Task<ActionResult> CreateChequeBottomMessage(ChequeBottomMessage chequeBottomMessage)
        {
            var result = await ChequeBottomMessageService.CreateAsync(chequeBottomMessage);

            if (result.Success)
            {
                return RedirectToAction("ChequeBottomMessages");
            }
            return View(chequeBottomMessage);
        }

        [HttpGet]
        [ActionName(nameof(ConfirmDeleteChequeBottomMessage))]
        public async Task<IActionResult> ConfirmDeleteChequeBottomMessage(int? id)
        {
            if (id.HasValue)
            {
                var result = await ChequeBottomMessageService.RemoveAsync(id.Value);
                if (result.Success)
                {
                    return RedirectToAction("ChequeBottomMessages");
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteChequeBottomMessages(int? id)
        {
            if (id.HasValue)
            {
                var result = await ChequeBottomMessageService.RemoveAsync(id.Value);
                if (result.Success)
                {
                    return RedirectToAction("ChequeBottomMessagess");
                }
            }
            return NotFound();
        }


        public async Task<ActionResult> EditChequeBottomMessage(int id)
        {
            var ChequeBottomMessageResult = await ChequeBottomMessageService.GetAsync(id);

            if (!ChequeBottomMessageResult.Success)
            {
                return NotFound();
            }
            return View(ChequeBottomMessageResult.Data);
        }

        // POST: ChequeBottomMessages/EditChequeBottomMessage
        [HttpPost]
        public async Task<ActionResult> EditChequeBottomMessage(ChequeBottomMessage ChequeBottomMessage)
        {
            var result = await ChequeBottomMessageService.EditAsync(ChequeBottomMessage);

            if (result.Success)
            {
                return RedirectToAction("ChequeBottomMessages");
            }


            return View(ChequeBottomMessage);
        }
    }
}
