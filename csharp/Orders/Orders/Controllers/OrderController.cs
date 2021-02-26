using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace Orders
{
    public class OrderController
    {
        private readonly PaperDbContext context;

        private readonly ILogger<OrderController> logger;


        private readonly AsyncRetryPolicy retryPolicy;

        public OrderController(PaperDbContext context, ILogger<OrderController> logger, AsyncRetryPolicy retryPolicy)
        {
            this.context = context;
            this.logger = logger;
            this.retryPolicy = retryPolicy;
        }

        [FunctionName("ProcessEdiOrder")]
        public async Task<IActionResult> ProcessEdiOrder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/create")] HttpRequest req,
            CancellationToken cts,
            [Queue("error-queue", Connection = "StorageConnectionString")] IAsyncCollector<string> messages)
        {
            this.logger.LogInformation("C# HTTP trigger function processed a request.");

            var order = new EdiOrder
            {
                Id = 0,
                ClientId = 1,
                DeliveryId = null,
                Notes = "Test order",
                ProductCode = 1,
                Quantity = 11
            };

            try
            {
                var pollyContext = new Context().WithLogger(this.logger);
                var deliveryModel = new DeliveryModel();

                await this.retryPolicy.ExecuteAsync(
                    async ctx =>
                    {
                        EdiOrder savedOrder = (await this.context.EdiOrder.AddAsync(order, cts)).Entity;
                        await this.context.SaveChangesAsync(cts);

                    }, pollyContext);

                // throw new ArgumentNullException("Name cannot be null or empty");

                return new OkObjectResult($"Order processed and completed with delivery {deliveryModel.Id}");
            }
            catch (Exception exception)
            {
                await messages.AddAsync(JsonConvert.SerializeObject(order), cts);
                this.logger.LogError("Delivery model exception: ", exception);
                this.logger.LogError("Delivery model exception Stacktrace: ", exception.StackTrace);
            }

            return new BadRequestObjectResult("Order cannot be processed.");
        }


        //[FunctionName("DataRestoreFunction")]
        //public async Task DataRestoreFunction(
        //    [QueueTrigger("error-queue", Connection = "StorageConnectionString")]
        //    string message,
        //    CancellationToken cts)
        //{
        //    var order = JsonConvert.DeserializeObject<EdiOrder>(message);
        //    throw new ArgumentNullException("Name cannot be null or empty");
        //    EdiOrder savedOrder = (await this.context.EdiOrder.AddAsync(order, cts)).Entity;
        //    await this.context.SaveChangesAsync(cts);
        //}    
        
        //[FunctionName("DataRestoreFunctionPoison")]
        //public async Task DataRestoreFunctionPoison(
        //    [QueueTrigger("error-queue-poison", Connection = "StorageConnectionString")]
        //    string message,
        //    CancellationToken cts)
        //{
        //    var order = JsonConvert.DeserializeObject<EdiOrder>(message);
        //    throw new ArgumentNullException("Name cannot be null or empty");
        //    EdiOrder savedOrder = (await this.context.EdiOrder.AddAsync(order, cts)).Entity;
        //    await this.context.SaveChangesAsync(cts);
        //}
    }
}
