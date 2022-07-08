# FS.TimeTracking.Report.Client.Api.ActivityReportApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ActivityReportGenerateActivityReport**](ActivityReportApi.md#activityreportgenerateactivityreport) | **POST** /api/report/v1/ActivityReport/GenerateActivityReport | Generates a report. |
| [**ActivityReportGenerateActivityReportPreview**](ActivityReportApi.md#activityreportgenerateactivityreportpreview) | **POST** /api/report/v1/ActivityReport/GenerateActivityReportPreview | Generates a report. |

<a name="activityreportgenerateactivityreport"></a>
# **ActivityReportGenerateActivityReport**
> void ActivityReportGenerateActivityReport (ActivityReportDto? activityReportDto = null)

Generates a report.

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
    public class ActivityReportGenerateActivityReportExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new ActivityReportApi(httpClient, config, httpClientHandler);
            var activityReportDto = new ActivityReportDto?(); // ActivityReportDto? | Source for the report. (optional) 

            try
            {
                // Generates a report.
                apiInstance.ActivityReportGenerateActivityReport(activityReportDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ActivityReportApi.ActivityReportGenerateActivityReport: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ActivityReportGenerateActivityReportWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Generates a report.
    apiInstance.ActivityReportGenerateActivityReportWithHttpInfo(activityReportDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ActivityReportApi.ActivityReportGenerateActivityReportWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **activityReportDto** | [**ActivityReportDto?**](ActivityReportDto?.md) | Source for the report. | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="activityreportgenerateactivityreportpreview"></a>
# **ActivityReportGenerateActivityReportPreview**
> void ActivityReportGenerateActivityReportPreview (ActivityReportDto? activityReportDto = null)

Generates a report.

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
    public class ActivityReportGenerateActivityReportPreviewExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new ActivityReportApi(httpClient, config, httpClientHandler);
            var activityReportDto = new ActivityReportDto?(); // ActivityReportDto? | Source for the report. (optional) 

            try
            {
                // Generates a report.
                apiInstance.ActivityReportGenerateActivityReportPreview(activityReportDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ActivityReportApi.ActivityReportGenerateActivityReportPreview: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ActivityReportGenerateActivityReportPreviewWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Generates a report.
    apiInstance.ActivityReportGenerateActivityReportPreviewWithHttpInfo(activityReportDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ActivityReportApi.ActivityReportGenerateActivityReportPreviewWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **activityReportDto** | [**ActivityReportDto?**](ActivityReportDto?.md) | Source for the report. | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json-patch+json, application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Success |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

