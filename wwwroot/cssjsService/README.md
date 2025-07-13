# Service Management CSS and JavaScript

This directory contains the CSS and JavaScript files for the service management functionality in the Pet Spa System application.

## Directory Structure

```
cssjsService/
├── css/                # CSS files
│   ├── service-common.css      # Common styles for all service management pages
│   ├── service-dashboard.css   # Dashboard specific styles
│   ├── service-list.css        # Service list page styles
│   ├── service-detail.css      # Service detail page styles
│   ├── service-form.css        # Styles for add/edit service forms
│   └── service-category.css    # Category management styles
├── js/                 # JavaScript files
│   ├── service-common.js       # Common functions for all service pages
│   ├── service-dashboard.js    # Dashboard specific functionality
│   ├── service-list.js         # Service list functionality
│   ├── service-detail.js       # Service detail page functionality
│   ├── service-form.js         # Add/edit service form functionality
│   └── service-category.js     # Category management functionality
└── README.md           # Documentation
```

## Files and Purposes

### CSS Files

- **service-common.css**: Contains shared styles used across all service management pages such as colors, card styling, tables, buttons, navigation tabs, and responsive adjustments.
- **service-dashboard.css**: Specific styles for the service dashboard, including stats cards, charts, and dashboard-specific components.
- **service-list.css**: Styles for the service listing page, including table styling, filters, and action buttons.
- **service-detail.css**: Styles for the service detail page, including tabs, charts, and information display.
- **service-form.css**: Shared styles for both the Add Service and Edit Service forms.
- **service-category.css**: Styles specific to category management.

### JavaScript Files

- **service-common.js**: Common utilities and functions used across all service pages, including alert handling, data formatting, and component initialization.
- **service-dashboard.js**: Dashboard-specific functionality, such as chart initialization and dashboard-specific table configurations.
- **service-list.js**: Service listing functionality, including table configuration, search/filter handling, and status change confirmations.
- **service-detail.js**: Detail page functionality, including tab handling, charts, and related information display.
- **service-form.js**: Form handling for both Add Service and Edit Service pages, including validation, image preview, and form submission.
- **service-category.js**: Category management functionality, including table configuration and category operations.

## Usage

In the view files, include the appropriate CSS and JS files:

```cshtml
@section Styles {
    <link rel="stylesheet" href="~/cssjsService/css/service-common.css" />
    <link rel="stylesheet" href="~/cssjsService/css/service-specific.css" />
}

@section Scripts {
    <script src="~/cssjsService/js/service-common.js"></script>
    <script src="~/cssjsService/js/service-specific.js"></script>
}
```

Replace `service-specific.css` and `service-specific.js` with the specific files needed for the page.

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
