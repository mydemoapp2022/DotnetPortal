---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Runtime"
labels: 'ADR'
---

# ADR-001 - Runtime

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
dotnet, runtime, LTS, .NET 10, .NET 8, framework

## Context

### Problem Statement
The project requires a runtime framework that is supported by the agency and will provide long-term stability, security, and maintainability. We need to choose a version of .NET that balances these factors while aligning with agency preferences.

### Required Outcomes
- Align with agency’s preference for .NET.
- Use a long-term support (LTS) version to minimize upgrade and security risk.
- Ensure the runtime is modern enough to support project requirements.
- Maintain supportability throughout the expected project lifecycle.

### Options
1. **.NET 8**
   - **Description:** Current LTS version at the start of the project.
   - **Pros:** Already stable, supported, and widely used; agency familiarity.
   - **Cons:** Will become slightly outdated sooner than .NET 10; fewer features than the upcoming LTS.
   - **Considerations:** May require an upgrade sooner if project lifecycle extends past .NET 8 support.

2. **.NET 10**
   - **Description:** Latest LTS version at project start and will remain current LTS for the foreseeable future.
   - **Pros:** Longest LTS support window; newest features; reduced need for upgrades during project lifecycle.
   - **Cons:** Slightly less community adoption at project start compared to .NET 8.
   - **Considerations:** Aligns with agency desire to use the most up-to-date LTS framework.

## Impact
This decision impacts all development staff working on the project, the CI/CD pipeline configuration, and potentially downstream services that interact with the project. At the enterprise level, choosing the latest LTS ensures long-term security and maintainability, minimizing risks associated with unsupported frameworks.

## Related ADRs
  

## Security
Using a current LTS version ensures ongoing security patches and support. No known security blockers exist with .NET 10, but standard security review procedures should be followed during development.

## Recommendation
We recommend **.NET 10** as the runtime framework. It provides the longest LTS support window, ensures project longevity without forced upgrades, and aligns with agency preferences for modern, maintainable, and secure software.

## Consequences
- All development, testing, and deployment pipelines will target .NET 10.  
- Project will remain supported and secure through its expected lifecycle.  
- If not approved, choosing .NET 8 would require a future upgrade sooner, potentially increasing project risk and maintenance overhead.

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 