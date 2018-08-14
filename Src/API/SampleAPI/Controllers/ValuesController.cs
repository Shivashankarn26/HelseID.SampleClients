using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace SampleAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ValuesController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return new JsonResult(User.Claims
                .Select(c => new { c.Type, c.Value }));
        }
    }
}
