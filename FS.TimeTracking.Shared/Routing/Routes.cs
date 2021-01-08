using System.Diagnostics.CodeAnalysis;

namespace FS.TimeTracking.Shared.Routing
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class Routes
    {
        public const string API_VERSION = "v1";
        private const string BASE_URL = null; // Set to null to use relative path.
        private const string API_ROUTE_PREFIX = BASE_URL + "api/" + API_VERSION + "/";

        public const string Root = BASE_URL + "";

        public class Information
        {
            private const string ROOT = API_ROUTE_PREFIX + nameof(Information) + "/";

            public const string GetProductName = ROOT + nameof(GetProductName);
            public const string GetProductVersion = ROOT + nameof(GetProductVersion);
            public const string GetProductCopyright = ROOT + nameof(GetProductCopyright);
        }

        public class OpenApi
        {
            public const string ROOT = BASE_URL + "openapi/";

            public const string OpenApiUi = ROOT;
            public const string OpenApiSpec = "openapi.json";
        }

        public class Swagger
        {
            public const string ROOT = BASE_URL + "swagger/";

            public const string SwaggerUi = ROOT;
        }

#if DEBUG
        public class DevTest
        {
            private const string ROOT = API_ROUTE_PREFIX + nameof(DevTest) + "/";

            public const string TestMethod = ROOT + nameof(TestMethod);
            public const string LongRunningOperation = ROOT + nameof(LongRunningOperation);
        }
#endif
    }
}
