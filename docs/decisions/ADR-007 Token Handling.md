---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Token Handling"
labels: 'ADR'
---

# ADR-007 - Token Handling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Authentication, token, Okta, cookie, session, security, OIDC

## Context

### Problem Statement
After authenticating users via Okta (ADR-005), the application receives a token that must be stored and transmitted securely for subsequent requests. The token handling approach must balance security, usability, and operational simplicity.

### Required Outcomes
- Securely store and transmit the authentication token.  
- Prevent unauthorized access or tampering.  
- Support consistent integration with the web application and backend services.  
- Align with standard web security practices.

### Options
1. **Store Token in Browser Cookie**
   - **Description:** Store the authentication token in a browser cookie, with `HttpOnly` and `Secure` flags to prevent access from JavaScript and ensure it is transmitted only over HTTPS.  
   - **Pros:** Simple implementation; aligns with common web authentication practices; browser handles token transmission automatically.  
   - **Cons:** Exposes token to potential CSRF attacks if not mitigated.  
   - **Considerations:** Additional protections like CSRF tokens or SameSite cookie attributes can mitigate risks. Current security assessment indicates this approach is sufficient for the token sensitivity level.

2. **Store Token in Server-Side Session**
   - **Description:** Keep the token on the server tied to a session, never exposing it to the client.  
   - **Pros:** Maximum security; token never leaves server memory.  
   - **Cons:** Increases server complexity; requires session management infrastructure; may be overkill for current token sensitivity.  
   - **Considerations:** Provides higher security but at additional operational cost.

## Impact
This decision affects how all authenticated requests are made from the client to backend services. Developers will implement token handling according to the selected approach, and the security team should validate mitigation strategies like CSRF protections.

## Related ADRs
ADR-005 (Authentication Tooling)

## Security
Storing the token in an `HttpOnly` and `Secure` cookie prevents client-side script access and ensures transmission only over HTTPS. Additional mitigations (e.g., SameSite cookies, CSRF protections) should be applied. The current security assessment indicates this level of protection is appropriate for the token in use.

## Recommendation
We recommend **storing the token in a browser cookie** with `HttpOnly` and `Secure` flags. This approach balances security, simplicity, and operational requirements, and is sufficient for the token sensitivity in the current authentication model.

## Consequences
- Browser will handle token transmission automatically.  
- CSRF protections and SameSite cookie settings will be required to mitigate potential attacks.  
- Server-side session storage is not required for this token at this time, simplifying infrastructure and code.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 