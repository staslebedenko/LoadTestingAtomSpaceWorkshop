using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Orders
{
    public static class LoaderActivation
    {
        [FunctionName("LoaderActivation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "loaderio-8f99b9a1d1178d")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Loader.io validation triggered.");
            return (ActionResult)new OkObjectResult($"loaderio-8f99597d1d1178d");
        }
    }
}
