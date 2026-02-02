---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Session Management"
labels: 'ADR'
---

# ADR-008 - Session Management

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Session, state management, SQL Server, distributed session, cookies, ASP.NET, stateless

## Context

### Problem Statement
The application requires session management to maintain user state across requests in a stateless web environment. The solution must support the agency’s infrastructure and follow supported ASP.NET patterns.

### Required Outcomes
- Maintain user session state reliably across stateless machines.  
- Ensure sessions are consistent and accessible from any server handling requests.  
- Align with agency-provided infrastructure and best practices.  
- Minimize client-side overhead and security risks.

### Options
1. **Persistent Session Store (SQL Server)**
   - **Description:** Use the agency-provided SQL Server instance to store session data. This aligns with the ASP.NET supported method for distributed session management.  
   - **Pros:** Centralized session storage; supports multiple stateless servers; secure and maintainable; aligns with agency standards.  
   - **Cons:** Requires database infrastructure and connection management.  
   - **Considerations:** Recommended for applications running in distributed environments to ensure consistency and reliability.

2. **Client-Side Session (Cookies)**
   - **Description:** Store session data in cookies and pass it with each request.  
   - **Pros:** No server-side session storage required; simplifies infrastructure.  
   - **Cons:** Limited size; potential security risks; increased client-side transmission; does not support large or sensitive session data reliably.  
   - **Considerations:** Suitable only for small, non-sensitive session data and single-server scenarios.

## Impact
This decision impacts all users interacting with the application and all servers handling requests. Using a centralized session store ensures consistent behavior, reliability, and compliance with agency infrastructure standards.

## Related ADRs
ADR-007 (Token Handling)

## Security
Session data is stored in SQL Server on the agency’s secure infrastructure, reducing exposure of sensitive data to the client. Standard ASP.NET session security practices (e.g., encryption, secure cookies) will be applied.

## Recommendation
We recommend using a **persistent session store in the agency-provided SQL Server**. This approach provides reliable, centralized session management across stateless servers and aligns with supported ASP.NET patterns and agency infrastructure.

## Consequences
- Session data will be centrally stored and accessible across multiple web servers.  
- Increased reliance on database availability and connectivity.  
- Client-side cookie storage is not required for session state, reducing security and size limitations.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 