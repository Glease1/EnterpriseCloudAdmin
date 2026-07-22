# EnterpriseCloudAdmin Suite

This repository serves as a centralized workspace for my Linux-based C# administration tools and infrastructure automation.

## Overview

The goal of this suite is to provide secure, native-execution tools for Linux system administration using .NET, alongside automated baseline configuration for Linux nodes.

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

## Infrastructure Automation

Automated baseline configuration for Linux nodes. To run the playbook:
```bash
cd ansible
ansible-playbook -i inventory.ini site.yml -K
Security Note
Tools in this suite interact with sensitive system files (such as /etc/passwd) and system binaries. They are intended for authorized administrators only. Ensure strict binary permissions are applied to any deployed versions to prevent unauthorized system modification.

Built by Glease1
