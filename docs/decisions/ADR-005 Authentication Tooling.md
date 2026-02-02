---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Authentication Tooling"
labels: 'ADR'
---

# ADR-005 - Authentication Tooling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Authentication, Okta, OIDC, Azure AD, identity provider, SSO, security

## Context

### Problem Statement
The project requires integration with the agency’s existing authentication provider, Okta, to manage user authentication and access control. The solution must be flexible enough to accommodate potential future changes in identity providers and support additional internal authentication scenarios.

### Required Outcomes
- Integrate with Okta for user authentication.  
- Maintain flexibility to support other identity providers if needed.  
- Enable a potential secondary authentication mechanism for internal staff via Azure AD.  
- Follow standards-based authentication practices to reduce vendor lock-in and increase maintainability.

### Options
1. **Okta-Specific Libraries**
   - **Description:** Use Okta-provided SDKs and libraries built on top of OIDC.  
   - **Pros:** Direct support for Okta features; faster integration with Okta-specific functionality.  
   - **Cons:** Ties the solution closely to Okta; harder to switch identity providers in the future.  
   - **Considerations:** Provides the quickest path to integration but reduces flexibility.

2. **Standard OIDC Libraries**
   - **Description:** Implement authentication using standard OIDC libraries compatible with Okta.  
   - **Pros:** Vendor-agnostic; enables future flexibility to switch identity providers; supports secondary authentication (e.g., Azure AD) scenarios.  
   - **Cons:** Slightly more setup and configuration compared to Okta-specific libraries.  
   - **Considerations:** Aligns with standards-based practices, supporting long-term maintainability and multi-provider scenarios.

## Impact
This decision impacts all development teams implementing authentication flows and identity management. It ensures that the system is flexible enough to support future identity provider changes and internal staff authentication requirements.

## Related ADRs
  

## Security
Using OIDC with Okta meets agency security standards for authentication. A standards-based implementation reduces vendor lock-in and allows alignment with enterprise security policies. Secondary authentication scenarios, such as Azure AD for internal staff, are also supported by this approach.

## Recommendation
We recommend using **standard OIDC libraries** for authentication integration with Okta. This approach provides the flexibility to support multiple identity providers, aligns with standards-based practices, and accommodates future internal staff authentication scenarios with Azure AD.

## Consequences
- Authentication flows will be implemented using standard OIDC protocols.  
- Future identity provider changes or multi-provider setups will be easier to accommodate.  
- Slightly more initial setup than Okta-specific libraries, but greater long-term maintainability and flexibility.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 