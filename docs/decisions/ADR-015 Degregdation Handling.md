---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Degradation Handling"
labels: 'ADR'
---

# ADR-015 - Degradation Handling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Degradation, resiliency, fault tolerance, service failure, health monitoring, user experience

## Context

### Problem Statement
Some backend services are mandatory for key application functionality. The application must define how it behaves when one or more of these services are unavailable or experiencing failures. Poor handling could lead to a broken user experience, lost work, or unnecessary system load.

### Required Outcomes
- Provide users with reliable feedback when a required service is unavailable.  
- Maintain as much application functionality as possible without compromising integrity.  
- Reduce wasted user effort during service outages.  
- Enable proactive detection and recovery from service failures.

### Options
1. **Proactive Health Monitoring and Feature Fallback**
   - **Description:** Monitor backend services for availability and health (e.g., via heartbeat checks or consecutive failure thresholds). If a mandatory service is down, the associated feature is flagged as unavailable, and the user is informed before attempting operations. Other application functionality continues to operate.  
   - **Pros:** Prevents wasted user effort; preserves partial functionality; supports graceful degradation.  
   - **Cons:** Requires monitoring implementation and feature-level handling; complexity in tracking service status.  
   - **Considerations:** Allows configurable thresholds (e.g., consecutive failures over time) to avoid false positives.

2. **Immediate Error Propagation**
   - **Description:** Let the application attempt operations as usual; if a backend service fails, propagate the error to the user immediately.  
   - **Pros:** Simple to implement; no monitoring infrastructure needed.  
   - **Cons:** Users may start workflows that fail mid-process; poor user experience; higher support overhead.  
   - **Considerations:** Best for systems where partial failures are rare or non-critical.

3. **Full Application Lock / Offline Mode**
   - **Description:** If a mandatory backend service is down, block the user from using the application entirely until the service is restored.  
   - **Pros:** Prevents any wasted effort; ensures users never interact with partially functional features.  
   - **Cons:** Extremely disruptive; prevents access to unrelated features; may frustrate users.  
   - **Considerations:** Suitable only if all or most of the application depends critically on the failing service.

## Impact
This decision affects all users, particularly those interacting with features dependent on mandatory services. It also affects development and operational monitoring approaches.

## Related ADRs
ADR-009 (Activity Tracking) – may be used to log degradation events.  
ADR-014 (Downtime Support) – partial feature degradation could complement full downtime handling.

## Security
No direct security implications, though monitoring endpoints and degradation flags should follow standard access and logging controls to avoid exposing sensitive information.

## Recommendation
Decision pending. Current guidance is to consider **proactive health monitoring with feature-level fallback**, allowing unaffected parts of the application to continue functioning while informing users of temporary unavailability.

## Consequences
- Need to implement monitoring and feature flags for unavailable services.  
- Users may see temporary feature unavailability messages.  
- Simpler approaches (error propagation or full lockout) would be easier to implement but at the cost of user experience.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 