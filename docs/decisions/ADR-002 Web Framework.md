---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Web Framework"
labels: 'ADR'
---

# ADR-002 - Web Framework

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Blazor, MVC, Razor Pages, web framework, UI, components, interactivity

## Context

### Problem Statement
We need to select a web framework for the project that supports the transition from the existing application while enabling a maintainable, interactive, and componentized UI. The framework must handle form submissions, interactivity across inputs, uploads/downloads, and data grids effectively.

### Required Outcomes
- Deliver a maintainable and high-quality UI.  
- Support interactive components consistently across the application.  
- Allow faster development without compromising code quality.  
- Ensure long-term flexibility to accommodate future UI changes.  

### Options
1. **Razor Pages**
   - **Description:** Page-focused framework suitable for simple form-based apps.  
   - **Pros:** Simple, lightweight, familiar; good for form submissions.  
   - **Cons:** Limited componentization and interactivity; not ideal for complex, interactive UIs.  
   - **Considerations:** Could be faster for simple screens but may limit future flexibility.

2. **MVC**
   - **Description:** Model-View-Controller framework suitable for apps with business logic.  
   - **Pros:** Handles complex logic well; widely used; supports form submissions and page rendering.  
   - **Cons:** Componentization and interactive UI development can be slower; more boilerplate for highly interactive elements.  
   - **Considerations:** Could deliver the current app functionality but may slow down future interactive development.

3. **Blazor**
   - **Description:** Component-based framework for building interactive web apps using C#.  
   - **Pros:** Supports rich interactivity, reusable components, data binding, and consistent behavior across inputs, forms, and data grids; enables faster delivery for complex UI.  
   - **Cons:** Slightly newer technology; may require team ramp-up.  
   - **Considerations:** Best aligns with long-term vision of componentized, interactive UI while still supporting current form-heavy workflows.

## Impact
This decision affects all front-end and full-stack developers on the project. It influences architecture patterns, component structure, testing approach, and deployment. At the enterprise level, adopting Blazor may also guide future web projects toward componentized, interactive designs.

## Related ADRs
  

## Security
Blazor supports standard ASP.NET security practices, including authentication, authorization, and anti-forgery protections. No additional security concerns are expected beyond normal web application practices.

## Recommendation
We recommend **Blazor** as the web framework. While MVC could accomplish the same functional requirements, Blazor better supports the interactive, componentized UI envisioned for the future. It enables faster, higher-quality development with a consistent user experience across forms, inputs, uploads/downloads, and data grids.

## Consequences
- All UI development will follow Blazor component patterns.  
- Development teams may require upskilling in Blazor concepts.  
- Project delivery can proceed with higher velocity and maintainable, reusable components.  
- If not approved, MVC could be used, but interactive features and long-term componentization may take longer and be more complex to implement.

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 