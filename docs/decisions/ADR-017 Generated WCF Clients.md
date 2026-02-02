---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Generated WCF Clients"
labels: 'ADR'
---

# ADR-017 - Generated WCF Clients

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
WCF, client generation, code generation, dotnet-svcutil, WSDL, service contracts

## Context

### Problem Statement
The application must interact with backend WCF services. Client code needs to be generated from WSDL/service contracts. There are multiple options for generating and managing this client code, each with implications for source control, build process, and maintenance.

### Required Outcomes
- Ensure client code is always consistent with backend service contracts.  
- Avoid checking in generated code unnecessarily to reduce noise in version control.  
- Track meaningful changes to service contracts (WSDL) rather than generated output.  
- Integrate generation with the build process where possible.  

### Options
1. **Generate via Visual Studio “Add Connected Service”**
   - **Description:** Use Visual Studio menus to connect to a WCF service and generate client code. Generated code is usually checked into source control.  
   - **Pros:** Easy to use; visual tooling; works out of the box.  
   - **Cons:** Generated code checked into source control creates noise; changes in generated code are harder to track meaningfully; not easily automated in CI/CD.  
   - **Considerations:** Requires manual regeneration when WSDL changes; harder to enforce consistency in automated builds.

2. **Generate Programmatically via CLI (`dotnet-svcutil`)**
   - **Description:** Use `dotnet-svcutil` as part of the build process to generate WCF client code automatically. Only the WSDL/service contract is checked into source control.  
   - **Pros:** Tracks meaningful changes via WSDL; no generated code in repository; integrates cleanly into CI/CD; ensures consistency on every build.  
   - **Cons:** Slightly longer build times; requires setting up tooling in the build pipeline; developers must run generation locally.  
   - **Considerations:** Provides a cleaner, maintainable workflow while still leveraging the same underlying tooling as “Add Connected Service.”

## Impact
All developers and build pipelines must have the necessary tooling (`dotnet-svcutil`) installed. WSDL files must be maintained in source control and updated when backend services change. Generated client code is transient and produced at build time.

## Related ADRs
  

## Security
No direct security implications beyond the need to ensure WSDLs and generated code do not contain sensitive credentials or configuration.  

## Recommendation
We recommend **generating WCF client code programmatically via `dotnet-svcutil` during the build process**, checking only the WSDL/service contracts into source control. This ensures meaningful tracking of changes, integrates cleanly with CI/CD, and avoids noise in version control from generated files.

## Consequences
- Build process takes slightly longer due to client generation.  
- Developers must ensure tooling is installed locally.  
- Only WSDL/service contract changes are tracked in source control.  
- Generated code is always up to date with the service contract.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 