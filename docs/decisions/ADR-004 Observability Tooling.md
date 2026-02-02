---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Observability Tooling"
labels: 'ADR'
---

# ADR-004 - Observability Tooling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Observability, monitoring, Dynatrace, logging, metrics, tracing

## Context

### Problem Statement
The project requires observability tooling to monitor performance, diagnose issues, and maintain operational visibility. The agency has standardized on Dynatrace, and integration with this tool is required.

### Required Outcomes
- Enable monitoring, logging, and performance tracking consistent with agency practices.  
- Ensure compatibility with existing Dynatrace dashboards and alerting.  
- Support debugging and issue resolution efficiently across the application.  

### Options
1. **Dynatrace (Agency Standard)**
   - **Description:** Integrate the application with Dynatrace, the agency’s chosen observability tool.  
   - **Pros:** Aligns with agency standards; fully supported by existing operational teams; provides full-stack observability.  
   - **Cons:** Requires compliance with agency configuration and integration processes.  
   - **Considerations:** Limited choice; integration is mandatory for compliance.

2. **Alternative Tools (e.g., Prometheus, Grafana, New Relic)**
   - **Description:** Consider other monitoring and observability tools.  
   - **Pros:** Potentially more flexible or feature-rich for some use cases.  
   - **Cons:** Not supported by the agency; would require additional approvals; could create operational complexity.  
   - **Considerations:** Not feasible given agency standardization and policy.

## Impact
This decision affects development and operations teams who will implement monitoring and alerting. Ensures consistent operational oversight across projects and aligns with enterprise-wide observability practices.

## Related ADRs
  

## Security
Using Dynatrace complies with the agency’s existing security and data policies. No additional security concerns are anticipated, as the platform is already vetted for use within the agency environment.

## Recommendation
We recommend **Dynatrace** as the observability tool. Integration with Dynatrace ensures compliance with agency policies, allows for consistent monitoring, and leverages existing operational support.

## Consequences
- Development teams must integrate and instrument the application according to Dynatrace standards.  
- Alternative observability tools will not be adopted at this time.  
- Monitoring and alerting will be consistent with agency practices, supporting reliable operations and incident response.

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 