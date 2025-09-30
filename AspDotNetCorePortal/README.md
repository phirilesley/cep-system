# Citizen Engagement Portal - ASP.NET Core 8 MVC

A comprehensive web-based platform that facilitates seamless communication between citizens and local government authorities for reporting and tracking community issues.

## 🏗️ **Architecture**

- **Framework**: ASP.NET Core 8 MVC
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with role-based access
- **UI**: Bootstrap 5 with Font Awesome icons
- **Email**: SendGrid integration for notifications
- **Language**: C#

## 🚀 **Features**

### Core Functionality
- **User Management**: Role-based authentication (Citizen, Admin, Official)
- **Issue Reporting**: Comprehensive issue submission with categories and location
- **Media Upload**: Image and video support for issues
- **Issue Tracking**: Real-time status updates and progress monitoring
- **Comment System**: Discussion and feedback on issues
- **Notifications**: Email notifications for issue updates
- **Analytics**: Comprehensive reporting and dashboard for administrators

### User Roles
- **Citizens**: Report issues, track progress, add comments
- **Administrators**: Full system access, user management, analytics
- **Officials**: Issue assignment and resolution management

## 📋 **Prerequisites**

- .NET 8 SDK
- Visual Studio 2022 or VS Code
- SQLite (included with .NET)

## 🛠️ **Setup Instructions**

### 1. **Clone the Repository**
```bash
git clone <repository-url>
cd CitizenEngagementPortal
```

### 2. **Restore Dependencies**
```bash
dotnet restore
```

### 3. **Database Setup**
The application will automatically create and seed the database on first run. The database includes:

- **Default Categories**: Infrastructure, Sanitation, Health, Safety, Environment, Utilities
- **Default Users**:
  - Admin: `admin@cep.local` / `Admin123!`
  - Citizen: `citizen@example.com` / `Citizen123!`
- **Sample Issues**: Pre-populated for demonstration

### 4. **Run the Application**
```bash
dotnet run
```

The application will be available at `https://localhost:5001`

### 5. **Email Configuration (Optional)**
To enable email notifications, configure SendGrid in `appsettings.Development.json`:

```json
{
  "SendGrid": {
    "ApiKey": "your-sendgrid-api-key",
    "FromEmail": "noreply@yourdomain.com"
  }
}
```

## 🗄️ **Database Schema**

### Core Entities
- **User**: User accounts with role-based access
- **Issue**: Community issues with status tracking
- **IssueCategory**: Pre-defined issue categories
- **IssueMedia**: Image/video attachments for issues
- **Comment**: Discussion threads for issues
- **Notification**: System notifications and alerts

### Relationships
- Users → Issues (One-to-Many: Reported Issues)
- Users → Issues (One-to-Many: Assigned Issues)
- Issues → Media (One-to-Many)
- Issues → Comments (One-to-Many)
- Issues → Notifications (One-to-Many)

## 🎯 **Key Pages**

### Public Pages
- **Home**: Landing page with feature overview
- **Account/Register**: User registration
- **Account/Login**: User authentication

### Citizen Pages
- **Dashboard**: Personal issue overview and statistics
- **Issues/Create**: Report new issues
- **Issues/Index**: View and filter personal issues
- **Issues/Details**: Issue details with comments

### Admin Pages
- **Admin/Index**: Administrative dashboard
- **Admin/Issues**: Manage all issues
- **Admin/Analytics**: Comprehensive analytics and reporting
- **Admin/AssignIssue**: Assign issues to officials

## 🔧 **Configuration**

### Environment Variables
- `ConnectionStrings__DefaultConnection`: Database connection string
- `SendGrid__ApiKey`: SendGrid API key for email notifications
- `SendGrid__FromEmail`: Default sender email address
- `App__Url`: Application base URL

### User Roles and Permissions
- **Citizen**: Create issues, view own issues, add comments
- **Official**: View assigned issues, update status, add comments
- **Admin**: Full system access, user management, analytics

## 📊 **Analytics and Reporting**

The admin dashboard provides comprehensive analytics including:
- Issue statistics by category and status
- Resolution time metrics
- Monthly trend analysis
- User engagement metrics
- Top problem categories

## 🚨 **Troubleshooting**

### Common Issues

1. **Database Migration Errors**
   ```bash
   dotnet ef database update
   ```

2. **Permission Issues**
   - Ensure the application has write permissions for the database file
   - Check `wwwroot/uploads` directory permissions for file uploads

3. **Email Not Working**
   - Verify SendGrid API key configuration
   - Check firewall settings for SMTP ports

4. **Login Issues**
   - Ensure default users are created in the database
   - Check password requirements (minimum 6 characters)

### Logging
Check the console output for detailed error messages and debugging information.

## 🤝 **Contributing**

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 **License**

This project is licensed under the MIT License.

## 🎯 **Project Benefits**

- **Citizen Empowerment**: Easy issue reporting and tracking
- **Transparent Governance**: Full visibility into issue resolution
- **Efficient Administration**: Streamlined issue management
- **Data-Driven Decisions**: Comprehensive analytics and reporting
- **Community Engagement**: Improved communication channels

## 📞 **Support**

For support and questions, please open an issue in the repository or contact the development team.

---

**Built with ❤️ using ASP.NET Core 8 MVC**