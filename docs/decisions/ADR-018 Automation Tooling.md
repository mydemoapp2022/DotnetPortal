---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Automation Tooling"
labels: 'ADR'
---

# ADR-018 - Automation Tooling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Automation, testing, unit tests, integration tests, xUnit, bUnit, Playwright, Selenium, Cypress

## Context

### Problem Statement
The project requires a consistent and maintainable approach to automated testing, including unit tests, component tests, and end-to-end (E2E) tests. Tooling choices should align with the .NET ecosystem, minimize cognitive overhead for developers, and keep QA automation efforts aligned with engineering practices.

### Required Outcomes
- Support logical unit testing for backend and shared code.
- Enable component-level testing for Blazor UI.
- Provide a path for end-to-end testing of the full application.
- Prefer tooling aligned with the C#/.NET ecosystem.
- Avoid unnecessary additional platforms (e.g., Node.js) where possible.

### Options

#### Unit Testing Frameworks
1. **MSTest**
   - **Description:** Microsoft’s built-in testing framework.
   - **Pros:** Official support; integrated with Visual Studio.
   - **Cons:** Less expressive assertions; less popular in modern .NET projects.
   - **Considerations:** Viable but not preferred.

2. **NUnit**
   - **Description:** Mature and widely used .NET testing framework.
   - **Pros:** Rich feature set; strong community history.
   - **Cons:** Slightly heavier syntax; less common in newer projects.
   - **Considerations:** Acceptable alternative.

3. **xUnit**
   - **Description:** Modern, lightweight .NET testing framework.
   - **Pros:** Clean syntax; strong community adoption; excellent tooling support.
   - **Cons:** None significant.
   - **Considerations:** Preferred standard for logical unit tests.

#### Blazor Component Testing
1. **bUnit**
   - **Description:** Blazor-specific testing library built on top of xUnit.
   - **Pros:** Purpose-built for Blazor; integrates naturally with xUnit; stays within C# ecosystem.
   - **Cons:** Limited strictly to component-level testing.
   - **Considerations:** Best fit for validating Blazor UI logic and rendering.

#### End-to-End (E2E) Testing
1. **Playwright**
   - **Description:** Modern E2E testing framework with strong browser automation support.
   - **Pros:** Reliable; supports modern web apps; .NET bindings available; good fit for CI/CD.
   - **Cons:** Requires additional setup; tooling is not native to ASP.NET.
   - **Considerations:** Preferred direction for E2E testing despite setup complexity.

2. **Selenium**
   - **Description:** Long-standing browser automation framework.
   - **Pros:** Mature; widely supported; works with C#.
   - **Cons:** More brittle; slower; heavier test maintenance.
   - **Considerations:** Acceptable fallback but not preferred.

3. **Cypress**
   - **Description:** Popular JavaScript-based E2E testing framework.
   - **Pros:** Strong developer experience in JavaScript ecosystems.
   - **Cons:** Requires Node.js; introduces additional language and platform complexity.
   - **Considerations:** Not preferred due to desire to avoid Node and keep tooling aligned with C#.

## Impact
This decision affects developers, QA automation engineers, and CI/CD pipelines. Standardizing on C#-centric tooling reduces context switching, improves maintainability, and keeps development and QA aligned.

## Related ADRs
ADR-002 (Web Framework – Blazor)  
ADR-012 (Deployments)

## Security
Automated testing tools do not introduce direct security risks but must be configured to avoid exposing credentials or sensitive data in test environments.

## Recommendation
We recommend:
- **xUnit** for logical unit testing.
- **bUnit** for Blazor component testing.
- **Playwright** as the preferred direction for end-to-end testing, acknowledging additional setup requirements.

This approach keeps the automation strategy largely within the **.NET/C# ecosystem**, minimizes tooling sprawl, and aligns developers and QA automation around a consistent stack.

## Consequences
- Developers and QA engineers will focus primarily on C#-based testing tools.
- Additional setup and maintenance is required for Playwright integration.
- Node-based tooling (e.g., Cypress) is intentionally avoided to reduce complexity.
- A cohesive and maintainable automation strategy is established across test layers.

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\]
 