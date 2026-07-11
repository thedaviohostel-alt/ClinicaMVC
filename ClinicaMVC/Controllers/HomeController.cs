using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaMVC.Controllers
{
    [Authorize] // Requiere haber iniciado sesión (cualquier rol)
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
