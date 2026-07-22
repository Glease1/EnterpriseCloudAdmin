# EnterpriseCloudAdmin Suite

This repository serves as a centralized workspace for my Linux-based C# administration tools. It contains my primary administrative application, alongside the foundational proof-of-work projects that facilitated its development.

## Overview

The goal of this suite is to provide secure, native-execution tools for Linux system administration using .NET. By combining these projects, I am able to maintain a clear trajectory of my development process—from basic C# Linux interaction to robust, audit-ready system tools.

## Projects

### 1. [UserAdminTool](./UserAdminTool/)
*My flagship administrative CLI application.*
Designed to manage local Linux user accounts with auditing and native binary integration. This is the primary project for enterprise-grade account lifecycle management.

### 2. [HealthMonitor](./HealthMonitor/)
*Foundational Project.*
A C# utility focused on system monitoring and file-based logging. This project was essential for mastering filesystem interaction and robust logging patterns in Linux.

### 3. [LinuxCalculator](./LinuxCalculator/)
*Proof-of-Work.*
A CLI application used to verify .NET SDK compilation and basic C# syntax on Linux. This project confirmed my environment setup and established the standard build workflow for the rest of the suite.

---

## Getting Started

Each project is self-contained. Navigate into the specific directory and use the standard .NET CLI commands:

```bash
# Example
cd UserAdminTool
dotnet run
Security Note
Tools in this suite interact with sensitive system files (such as /etc/passwd) and system binaries. They are intended for authorized administrators only. Ensure strict binary permissions are applied to any deployed versions to prevent unauthorized system modification.

Built by Glease1
