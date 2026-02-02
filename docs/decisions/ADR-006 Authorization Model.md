---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Authorization Model"
labels: 'ADR'
---

# ADR-006 - Authorization Model

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Authorization, RBAC, ABAC, LoginService, synthetic key, access control

## Context

### Problem Statement
After user authentication via Okta (ADR-005), the system needs to determine what actions and resources the current user is authorized to access. The authorization model must integrate with backend services and support role- and attribute-based access control.

### Required Outcomes
- Accurately determine user permissions and access rights based on backend service responses.  
- Support RBAC and ABAC policies for flexibility in defining access rules.  
- Maintain a secure and auditable mechanism for authorizing users.  
- Ensure seamless integration with the LoginService for retrieving user authorization data.

### Options
1. **Backend-Driven Authorization**
   - **Description:** After authentication, extract the user ID from the token and send it to the LoginService, which returns a synthetic key and authorization data. RBAC/ABAC rules are applied based on this response.  
   - **Pros:** Centralizes authorization logic; ensures consistent access decisions; aligns with enterprise backend practices.  
   - **Cons:** Dependent on the LoginService being available and responsive.  
   - **Considerations:** Requires integration with LoginService and proper caching or fallback handling to prevent authorization failures.

2. **Client-Side Authorization**
   - **Description:** Implement authorization rules directly in the client application based on token claims.  
   - **Pros:** Reduces backend calls for authorization.  
   - **Cons:** Increases risk of inconsistent enforcement; harder to maintain and audit; does not align with agency practices.  
   - **Considerations:** Not suitable for this project due to centralized backend authorization requirement.

## Impact
This decision affects all applications and services consuming user identity information. All access control decisions will be determined using RBAC/ABAC rules in conjunction with the LoginService response. Ensures enterprise-standard access control and compliance with agency policies.

## Related ADRs
ADR-005 (Authentication Tooling)

## Security
Authorization is based on a synthetic key provided by the LoginService, minimizing exposure of sensitive user data. RBAC and ABAC enforcement ensures that users can only access authorized resources, reducing the risk of unauthorized access.

## Recommendation
We recommend using **backend-driven RBAC/ABAC authorization via the LoginService**. This approach provides a secure, centralized mechanism for determining user permissions and ensures consistent enforcement across the application.

## Consequences
- All access control decisions are dependent on LoginService responses.  
- Integration with LoginService is required for all services that enforce authorization.  
- RBAC/ABAC rules must be maintained in sync with enterprise policies to ensure correct access enforcement.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 