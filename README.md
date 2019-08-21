# CSharpWebProjects
A collection of basic web app demos.

## ContosoUniversity
This is an ASP.NET MVC, C#, code-first database application.
It models a hypothetical school with students, professors, departments, courses, office-locations, budgets, etc.
Highlights:
1. Code-first database interface
2. Numerous database migrations demonstrating how to manage changes to a database structure
3. Intercepting transient database errors with automatic retry for superior user experience
4. Simulating transiet database errors for testing/debugging
5. Logging errors through logger interface class (for optimal scalability)
6. CRUD (create-read-update-delete) database operations for all major tables (types)
7. ViewModels

## ExcelAsSQLdataSource
This is an ASP.NET MVC, C#, database-first database application.
It models a hypothetical company financial analysis with thirteen fields (criteria)
Highlights:
1. Database-first database interface
2. Uploading/downloading files to/from the server
3. Decompressing Excel files
4. Reading and parsing XML directly from Excel files into database entities
5. Use of metadata type for validating data prior to saving to the database
6. Extension methods and reflection to read metadata from the metadata type
7. Detailed error reporting so that user knows exactly which lines/fields are causing import errors
8. ViewModels

## InsuranceQuoteDB
This is an ASP.NET MVC, C#, database-first database application.
It models a hypothetical auto insurance quote system.
Highlights:
1. Third Normal Form (3NF) database design
2. Business logic layer (specific to risk factors in the insurance field)
3. AJAX calls and scripts to update selection options as the user selects the make, model, etc.
4. User roles and permissions (including Visitor functionality)
5. Allows users to save quotes and update driving history
6. ViewModels

## StudentManagementSystem
This is a basic ASP.NET MVC, C#, database-first database application.
It models a hypothetical student database with basic CRUD functionality.
