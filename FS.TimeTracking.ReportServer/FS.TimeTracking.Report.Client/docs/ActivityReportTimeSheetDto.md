# FS.TimeTracking.Report.Client.Model.ActivityReportTimeSheetDto
Time sheet report grid data transfer object.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** | The unique identifier of the entity. | 
**StartDate** | **DateTime** | Gets or sets the start date. | 
**EndDate** | **DateTime?** | Gets or sets the end date. | [optional] 
**Duration** | **string** | Gets or sets the total working time. | [optional] 
**Comment** | **string** | Gets or sets the comment. | [optional] 
**Issue** | **string** | Gets or sets the related issue/ticket/... . | [optional] 
**CustomerTitle** | **string** | Gets or sets the customer title. | [optional] 
**ProjectTitle** | **string** | Gets or sets the project title. | [optional] 
**ActivityTitle** | **string** | Gets or sets the activity title. | [optional] 
**OrderTitle** | **string** | Gets or sets the order title. | [optional] 
**OrderNumber** | **string** | Gets or sets the order number. | [optional] 
**Billable** | **bool** | Indicates whether this item is billable. | 
**CustomerCompanyName** | **string** | Gets or sets the name of the customer&#39;s company. | [optional] 
**CustomerDepartment** | **string** | Gets or sets the customer&#39;s department. | [optional] 
**CustomerContactName** | **string** | Gets or sets the name of the customer&#39;s contact. | [optional] 
**GroupBy** | **string** | Gets or sets entity the report should grouped by. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

