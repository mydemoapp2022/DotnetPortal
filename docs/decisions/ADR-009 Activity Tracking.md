---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Activity Tracking"
labels: 'ADR'
---

# ADR-009 - Activity Tracking

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Activity tracking, auditing, logging, Oracle, user actions, compliance

## Context

### Problem Statement
The project must track user actions on the platform to comply with agency requirements for auditing and operational monitoring. The tracking system must integrate with the agency-provided infrastructure and follow their established activity tracking model.

### Required Outcomes
- Record all relevant user actions on the platform.  
- Store activity data in the agency-provided Oracle database.  
- Align with existing agency models for activity tracking and auditing.  
- Ensure the tracking solution is reliable, consistent, and auditable.

### Options
1. **Agency-Provided Oracle Activity Tracking**
   - **Description:** Implement activity tracking in accordance with the agency’s existing model, using the provided Oracle database.  
   - **Pros:** Fully compliant with agency requirements; leverages existing infrastructure; consistent with other agency systems.  
   - **Cons:** Limited flexibility in data storage and tracking format; dependent on agency-provided database.  
   - **Considerations:** No alternative options were evaluated due to compliance requirements.

## Impact
This decision affects all users and systems that require auditing of user actions. Ensures consistent, auditable tracking aligned with agency practices.

## Related ADRs
  

## Security
Activity data will be stored in the agency’s secure Oracle database. Access controls, encryption, and auditing will follow agency standards, reducing risk of unauthorized access or tampering.

## Recommendation
We recommend implementing **activity tracking using the agency-provided Oracle database** in accordance with their existing model. This approach ensures compliance, consistency, and reliability of tracked user actions.

## Consequences
- All user actions will be recorded in the Oracle database.  
- Integration with agency systems is required for proper tracking.  
- Alternative tracking methods are not used; compliance is ensured by adhering to agency standards.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 