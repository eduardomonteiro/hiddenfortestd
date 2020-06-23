using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Faces.WebMvc.Models;
using MassTransit;
using Faces.WebMvc.ViewModels;
using System.IO;
using Messaging.InterfacesConstants.Constants;
using Messaging.InterfacesConstants.Commands;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl;

        public HomeController(ILogger<HomeController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        [HttpGet]
        public IActionResult RegistredOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegistredOrder(OrderViewModel orderModel)
        {
            MemoryStream memory = new MemoryStream();

            using (var uploadedFile = orderModel.File.OpenReadStream())
            {
                await uploadedFile.CopyToAsync(memory);
            }

            orderModel.ImageData = memory.ToArray();
            orderModel.ImageUrl = orderModel.File.FileName;
            orderModel.OrderId = Guid.NewGuid();

            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMquri}" + $"{RabbitMqMassTransitConstants.ResgisterOrderCommandQueue}");
            var endPoint = await _busControl.GetSendEndpoint(sendToUri);

            await endPoint.Send<IRegisterOrderCoommand>(
                new
                {
                    orderModel.OrderId,
                    orderModel.UserEmail,
                    orderModel.ImageData,
                    orderModel.ImageUrl
                });

            return View("Thanks");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
