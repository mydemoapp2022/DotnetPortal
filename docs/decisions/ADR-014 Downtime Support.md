---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Downtime Support"
labels: 'ADR'
---

# ADR-014 - Downtime Support

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Downtime, maintenance, IIS, offline, batch processing, availability, planned maintenance

## Context

### Problem Statement
The application must be able to handle planned downtime, particularly for batch processes run by the agency that require the web application to be temporarily offline. The solution must allow users to be informed when the application is unavailable and prevent partial or inconsistent operations during downtime.

### Required Outcomes
- Provide a clear mechanism for marking the application as offline.  
- Align with agency infrastructure and operational practices.  
- Avoid complex custom solutions unless necessary.  

### Options
1. **Leverage IIS “App Offline” Feature**
   - **Description:** Use the IIS built-in mechanism where placing an `app_offline.htm` file in the root of the web application directory automatically takes the app offline and serves the content of that file to all users.  
   - **Pros:** Built-in, simple, reliable, and supported by IIS; agency is already familiar with it.  
   - **Cons:** Only supports full downtime; no granular feature toggling.  
   - **Considerations:** Matches agency’s batch processing requirements; minimal development effort required.

2. **Custom Offline Endpoint / Feature Toggle**
   - **Description:** Build an endpoint or feature in the application to put the app into “offline mode,” potentially returning a custom maintenance page while disabling normal functionality.  
   - **Pros:** Allows finer control over downtime; could show custom messages or partial availability.  
   - **Cons:** Adds complexity; requires development and testing; duplicating IIS functionality unnecessarily.  
   - **Considerations:** Could be implemented in the future if more nuanced downtime control is needed.

## Impact
This decision affects all users accessing the application during batch processing downtime. Using IIS ensures the application is reliably taken offline and users see a consistent message.

## Related ADRs
ADR-012 (Deployments)

## Security
No direct security implications. Standard IIS permissions should be applied to control access to the `app_offline.htm` file to prevent unauthorized downtime.

## Recommendation
We recommend **using IIS built-in “App Offline” feature** by placing an `app_offline.htm` file in the root of the web application when planned downtime is needed. This approach is simple, reliable, supported, and aligns with agency operational procedures.

## Consequences
- During downtime, the app will be fully offline and all requests will serve the `app_offline.htm` content.  
- No additional custom endpoints or features are required for planned downtime.  
- Granular or partial downtime control is not provided, but can be added later if required.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 