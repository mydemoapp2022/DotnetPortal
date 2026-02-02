---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Internationalization and Localization"
labels: 'ADR'
---

# ADR-010 - Internationalization and Localization

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
i18n, l10n, localization, internationalization, language, American English

## Context

### Problem Statement
The project needs to define how it handles internationalization (i18n) and localization (l10n). While previous discussions considered supporting multiple languages, the current requirement is limited to American English only.

### Required Outcomes
- Deliver the application in American English.  
- Avoid unnecessary complexity for multi-language support.  
- Ensure maintainability of the codebase without resource files or localization frameworks for other languages.

### Options
1. **Support Multiple Languages**
   - **Description:** Implement i18n/l10n with resource files or frameworks to allow multiple languages.  
   - **Pros:** Future-proof; supports potential language expansion.  
   - **Cons:** Adds development and maintenance overhead; unnecessary for current requirements.  
   - **Considerations:** Could be added later if requirements change.

2. **American English Only**
   - **Description:** Build the application to support only American English, without resource files or localization frameworks.  
   - **Pros:** Simpler implementation; reduces development and maintenance complexity; meets current requirements.  
   - **Cons:** Not ready for future language expansion without code changes.  
   - **Considerations:** Future i18n/l10n can be added if needed, but not required now.

## Impact
This decision affects all UI text, messages, labels, and documentation. Developers will implement all text in American English without resource file abstractions.

## Related ADRs
  

## Security
No security implications are associated with this decision.

## Recommendation
We recommend **supporting only American English** at this time. This approach meets the current requirements, avoids unnecessary complexity, and allows for future expansion if requested.

## Consequences
- All application text will be in American English.  
- Multi-language support is not implemented and would require changes if needed in the future.  
- Simpler development and maintenance without additional localization frameworks.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 