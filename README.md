Markdown
# UserAdminTool

UserAdminTool is a native Linux administrative CLI application written in C#. It is designed to manage local Linux user accounts—including account lifecycle and status monitoring—within an enterprise environment. The tool follows a "Native Execution" philosophy, interacting directly with Linux system binaries (useradd, userdel, chpasswd) to ensure high performance and minimal overhead.

## Key Features

* **Lifecycle Management**: Create, delete, lock, and unlock local Linux users.
* **Credential Handling**: Native integration for resetting user passwords.
* **Audit Logging**: Every administrative action is logged to audit.log with UTC timestamps.
* **System Reporting**: Provides a formatted list of users, including creation time, last login, and account status.

## Commands

| Command | Description |
| :--- | :--- |
| create | Adds a user and sets an initial password. |
| delete | Removes a user and purges their home directory. |
| reset | Updates a user's password. |
| lock | Locks an account. |
| unlock | Unlocks an account. |
| list | Generates a report of system users. |
| help | Displays available commands. |
| exit | Closes the application. |

## Security & Audit

Security is handled via the AuditDecorator pattern. All operations pass through an audit layer that logs results to a local file. This ensures an immutable record of system changes for compliance and troubleshooting.

## Requirements

* .NET 8.0 SDK (or higher) on the target node.
* Root/Sudo Privileges: Because this tool modifies system files, it must be executed with elevated permissions.

## Build and Run

### Development
```bash
dotnet run
Deployment
Bash
dotnet publish -c Release -o ./bin/publish
./bin/publish/UserAdminTool
Security Note
This tool interacts with /etc/passwd and system binaries. It is intended for authorized administrators only. Ensure binary permissions are restricted on the target Linux node to prevent unauthorized system modification.
