---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Deployments"
labels: 'ADR'
---

# ADR-012 - Deployments

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Deployment, CI/CD, GitHub Actions, automation, on-prem, pipelines

## Context

### Problem Statement
The project requires a deployment strategy for moving code from development through test and into production. The approach should be reliable, repeatable, and minimize manual intervention while supporting the agency’s on-prem infrastructure.

### Required Outcomes
- Ensure consistent deployments across all environments.  
- Reduce human error and manual effort.  
- Integrate with existing agency infrastructure and tools.  
- Support rollback and recovery if issues occur during deployment.

### Options
1. **Manual Deployments**
   - **Description:** Deploy code manually to each environment by operators.  
   - **Pros:** Simple to implement initially; allows human oversight for each deployment.  
   - **Cons:** Error-prone; time-consuming; difficult to scale; inconsistent between environments.  
   - **Considerations:** Not suitable for frequent or complex deployments.

2. **Automated CI/CD Deployments**
   - **Description:** Use GitHub Actions in conjunction with on-prem runners to automate deployment pipelines for all environments.  
   - **Pros:** Reliable and repeatable; reduces manual intervention; integrates with existing on-prem infrastructure; supports rollback strategies.  
   - **Cons:** Requires initial setup and pipeline maintenance; requires operational knowledge of CI/CD systems.  
   - **Considerations:** Aligns with modern DevOps practices and agency requirements for repeatable deployments.

## Impact
This decision affects developers, operations teams, and deployment processes. Automation improves consistency, reduces errors, and supports faster delivery cycles.

## Related ADRs
  

## Security
Automated pipelines can enforce security checks, approvals, and compliance controls as part of the deployment process, reducing risks associated with manual deployments.

## Recommendation
We recommend **automated CI/CD deployments using GitHub Actions and on-prem runners**. This approach ensures repeatable, reliable, and secure deployments while leveraging existing infrastructure.

## Consequences
- Deployment pipelines must be maintained and updated as application evolves.  
- Developers and operators need familiarity with CI/CD tooling.  
- Manual deployments are largely eliminated, reducing potential human errors.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 