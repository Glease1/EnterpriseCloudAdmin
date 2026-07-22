EnterpriseCloudAdmin: Linux Management Suite
This repository contains my development journey in creating native C# tools for headless Linux management. It documents the evolution from foundational exercises to complex system administration utilities.

Current Project: UserAdminTool
A native Linux administrative CLI application written in C# designed to manage local user accounts within an enterprise environment.

Philosophy: Uses "Native Execution" by interacting directly with Linux system binaries (useradd, userdel, chpasswd) for high performance.

Key Features: Account lifecycle management (create/delete/lock/unlock), credential resets, UTC-timestamped audit logging, and formatted system reporting.

Security: Implements the AuditDecorator pattern to ensure immutable audit logs for compliance and troubleshooting.

Requirements: .NET 8.0 SDK or higher; must be run with Root/Sudo privileges.

Learning Journey & Proof-of-Work
This repository also archives the projects that helped me build the skills necessary for the UserAdminTool:

HealthMonitor: An early exploration of system monitoring and file-based logging.

LinuxCalculator: A foundational C# implementation used to verify build and compilation processes in a Linux environment.
