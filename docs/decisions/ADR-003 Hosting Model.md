---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Hosting Model"
labels: 'ADR'
---

# ADR-003 - Hosting Model

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Hosting, on-premises, infrastructure, deployment, environment

## Context

### Problem Statement
The project needs a hosting model for deploying and running the application. The agency already maintains on-premises infrastructure, and the decision must align with existing operational capabilities.

### Required Outcomes
- Ensure the application runs reliably within agency-supported infrastructure.  
- Minimize additional operational complexity or costs.  
- Align with agency policies and preferences for deployment environments.

### Options
1. **On-Premises (Agency Infrastructure)**
   - **Description:** Deploy the application to the agency’s existing on-premises servers and infrastructure.  
   - **Pros:** Fully supported by agency operations; aligns with existing policies; avoids cloud-related compliance or cost considerations.  
   - **Cons:** Limited scalability compared to cloud; requires internal operational management.  
   - **Considerations:** No evaluation of cloud options; aligns directly with agency preference.

2. **Cloud Hosting**
   - **Description:** Deploy the application to a cloud provider.  
   - **Pros:** Potentially scalable and flexible; could reduce infrastructure management overhead.  
   - **Cons:** Not evaluated; not aligned with agency preference; may introduce compliance and cost considerations.  
   - **Considerations:** Rejected due to agency policy and lack of evaluation.

## Impact
This decision impacts deployment operations, DevOps processes, and ongoing system maintenance. Aligning with the agency’s on-prem infrastructure simplifies operational support and ensures compliance with existing policies.

## Related ADRs
  

## Security
Leveraging the agency’s on-prem infrastructure allows the project to inherit established security practices, policies, and monitoring. No additional cloud-specific security considerations are required.

## Recommendation
We recommend hosting the application on the agency’s **existing on-premises infrastructure**. This approach aligns with agency policy, avoids additional complexity, and ensures the system is fully supported operationally.

## Consequences
- Deployment and maintenance will follow existing on-premises procedures.  
- Cloud options were not evaluated and may be considered in the future if agency policy changes.  
- Project remains aligned with agency preferences and support capabilities.

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 