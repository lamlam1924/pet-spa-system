# Service Management CSS and JS Structure

This folder contains CSS and JavaScript files used by the Service Management section of the Pet Spa System.

## CSS Files

1. `service-common.css`: Common styles shared across all service management pages
   - Color variables
   - Button styles
   - Card styles
   - Utility classes

2. `service-dashboard.css`: Styles specific to the service dashboard page
   - Chart container styles
   - Statistics card styles
   - Badge styles

3. `service-list.css`: Styles specific to the service list page
   - Dashboard card styles
   - Table styles
   - Search bar styles
   - Service action button styles

4. `service-edit.css`: Styles specific to the service edit page
   - Form styles
   - Preview card styles
   - Input group styles
   - Change history timeline styles

## JavaScript Files

1. `service-utils.js`: Utility functions shared across all service management pages
   - Success and error message display
   - Currency formatting
   - Common initialization functions

2. `service-dashboard.js`: Scripts specific to the service dashboard page
   - Chart initialization and rendering
   - Dashboard metrics handling

3. `service-list.js`: Scripts specific to the service list page
   - List filtering and sorting functionality
   - Status change confirmation
   - Print functionality

4. `service-edit.js`: Scripts specific to the service edit page
   - Live preview updates
   - Form validation and submission
   - Reset functionality

5. `service-add.js`: Scripts specific to the service add page
   - Live preview updates as fields are filled
   - Form validation and reset functionality

## How to Use

1. Include the common CSS and JS files on all service management pages
2. Include page-specific CSS and JS files as needed
3. Pass any required data from the controller to the JS files using script blocks

Example:

```html
@section Styles {
    <link rel="stylesheet" href="~/cssjsservice/service-common.css" />
    <link rel="stylesheet" href="~/cssjsservice/service-dashboard.css" />
}

@section Scripts {
    <script src="~/cssjsservice/service-utils.js"></script>
    <script src="~/cssjsservice/service-dashboard.js"></script>
    <script>
        // Pass data to the external JS file
        var serviceBookingData = @Html.Raw(Json.Serialize(ViewBag.ServiceBookingData));
        var categoryDistributionData = @Html.Raw(Json.Serialize(ViewBag.CategoryDistributionData));
    </script>
}
```
