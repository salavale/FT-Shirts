using FTShirts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;

namespace FTShirts.Web.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
