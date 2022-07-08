# FS.TimeTracking.Report.Client.Api.InformationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**InformationGetProductCopyright**](InformationApi.md#informationgetproductcopyright) | **GET** /api/report/v1/Information/GetProductCopyright | Gets the copyright for the product. |
| [**InformationGetProductInformation**](InformationApi.md#informationgetproductinformation) | **GET** /api/report/v1/Information/GetProductInformation | Gets the name, version and copyright of the product. |
| [**InformationGetProductName**](InformationApi.md#informationgetproductname) | **GET** /api/report/v1/Information/GetProductName | Gets the name of the product. |
| [**InformationGetProductVersion**](InformationApi.md#informationgetproductversion) | **GET** /api/report/v1/Information/GetProductVersion | Gets the product version. |

<a name="informationgetproductcopyright"></a>
# **InformationGetProductCopyright**
> string InformationGetProductCopyright ()

Gets the copyright for the product.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FS.TimeTracking.Report.Client.Api;
using FS.TimeTracking.Report.Client.Client;
using FS.TimeTracking.Report.Client.Model;

namespace Example
{
    public class InformationGetProductCopyrightExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new InformationApi(httpClient, config, httpClientHandler);

            try
            {
                // Gets the copyright for the product.
                string result = apiInstance.InformationGetProductCopyright();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling InformationApi.InformationGetProductCopyright: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InformationGetProductCopyrightWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets the copyright for the product.
    ApiResponse<string> response = apiInstance.InformationGetProductCopyrightWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling InformationApi.InformationGetProductCopyrightWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="informationgetproductinformation"></a>
# **InformationGetProductInformation**
> ProductInformationDto InformationGetProductInformation ()

Gets the name, version and copyright of the product.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FS.TimeTracking.Report.Client.Api;
using FS.TimeTracking.Report.Client.Client;
using FS.TimeTracking.Report.Client.Model;

namespace Example
{
    public class InformationGetProductInformationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new InformationApi(httpClient, config, httpClientHandler);

            try
            {
                // Gets the name, version and copyright of the product.
                ProductInformationDto result = apiInstance.InformationGetProductInformation();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling InformationApi.InformationGetProductInformation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InformationGetProductInformationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets the name, version and copyright of the product.
    ApiResponse<ProductInformationDto> response = apiInstance.InformationGetProductInformationWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling InformationApi.InformationGetProductInformationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**ProductInformationDto**](ProductInformationDto.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="informationgetproductname"></a>
# **InformationGetProductName**
> string InformationGetProductName ()

Gets the name of the product.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FS.TimeTracking.Report.Client.Api;
using FS.TimeTracking.Report.Client.Client;
using FS.TimeTracking.Report.Client.Model;

namespace Example
{
    public class InformationGetProductNameExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new InformationApi(httpClient, config, httpClientHandler);

            try
            {
                // Gets the name of the product.
                string result = apiInstance.InformationGetProductName();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling InformationApi.InformationGetProductName: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InformationGetProductNameWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets the name of the product.
    ApiResponse<string> response = apiInstance.InformationGetProductNameWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling InformationApi.InformationGetProductNameWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="informationgetproductversion"></a>
# **InformationGetProductVersion**
> string InformationGetProductVersion ()

Gets the product version.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FS.TimeTracking.Report.Client.Api;
using FS.TimeTracking.Report.Client.Client;
using FS.TimeTracking.Report.Client.Model;

namespace Example
{
    public class InformationGetProductVersionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new InformationApi(httpClient, config, httpClientHandler);

            try
            {
                // Gets the product version.
                string result = apiInstance.InformationGetProductVersion();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling InformationApi.InformationGetProductVersion: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the InformationGetProductVersionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Gets the product version.
    ApiResponse<string> response = apiInstance.InformationGetProductVersionWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling InformationApi.InformationGetProductVersionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

