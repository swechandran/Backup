//namespace APISample.Repository
//{
//    public class EndPointMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IErrorLog _logger;

//        public EndPointMiddleware(RequestDelegate next, IErrorLog logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var endpoint = context.GetEndpoint();
//            if (endpoint == null)
//            {
//                _logger.Error("Invalid endpoint accessed.");
//                context.Response.StatusCode = StatusCodes.Status404NotFound;
//                await context.Response.WriteAsync("Endpoint not found.");
//                return;
//            }

//            _logger.Error($"Valid endpoint: {endpoint.DisplayName}");
//            await _next(context); // Call the next middleware in the pipeline
//        }
//    }
//}
