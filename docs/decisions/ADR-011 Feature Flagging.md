---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Feature Flagging"
labels: 'ADR'
---

# ADR-011 - Feature Flagging

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Feature flags, toggles, configuration, deployment, builds, AOT compilation, caching

## Context

### Problem Statement
The application needs a way to enable or disable features dynamically or at build/deploy time. The chosen approach should balance operational simplicity, performance, and the ability to manage features across distributed instances.

### Required Outcomes
- Enable or disable features without negatively impacting performance.  
- Ensure consistent feature exposure across distributed instances.  
- Minimize deployment overhead while allowing flexibility for toggling features.  
- Provide maintainable mechanisms for development and operations teams.

### Options
1. **Database Flags**
   - **Description:** Store feature flags in a database, querying them for each request to determine which features are enabled.  
   - **Pros:** Dynamic changes without redeploying; centralized control; easy to update feature state.  
   - **Cons:** Requires queries on each request or caching to avoid performance issues; distributed instances need synchronized caches.  
   - **Considerations:** Suitable for applications where features change frequently and real-time toggling is needed.

2. **Separate Builds / Hardcoded Features**
   - **Description:** Generate application builds with only the desired features enabled, using hardcoding or Ahead-of-Time (AOT) compilation.  
   - **Pros:** Simplifies runtime logic; minimal performance impact; features cannot be accidentally exposed.  
   - **Cons:** Deployment required for every feature change; reduces flexibility; multiple builds may be hard to maintain.  
   - **Considerations:** Provides strict control over features, but operationally heavier.

3. **Configuration-Based Feature Flags (Pending Decision)**
   - **Description:** Store feature flags in app configuration files, which can be edited to enable or disable features.  
   - **Pros:** Balances flexibility and simplicity; changes may not require full rebuilds; centralized configuration is easier to manage.  
   - **Cons:** Requires configuration reload logic; potential for inconsistent state across distributed instances if not managed carefully.  
   - **Considerations:** Expected to be the likely approach, balancing deployment overhead and runtime flexibility.

## Impact
This decision affects development and operations teams, deployment processes, and runtime performance. All feature toggling strategies must ensure consistent behavior across all instances of the application.

## Related ADRs
  

## Security
Feature flags may expose or hide functionality, so access control to modify flags must be secured. Configuration files or databases must be protected to prevent unauthorized changes.

## Recommendation
Decision pending. Current expectation is to use **configuration-based feature flags**, balancing flexibility, maintainability, and deployment simplicity, although **separate builds** could be considered for stricter control over production features.

## Consequences
- Feature toggling mechanism must be implemented and tested.  
- Configuration changes or deployments may be required depending on the approach eventually chosen.  
- Performance, caching, and consistency considerations must be accounted for.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 