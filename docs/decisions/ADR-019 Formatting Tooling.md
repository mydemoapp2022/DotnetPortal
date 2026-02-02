---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Formatting Tooling"
labels: 'ADR'
---

# ADR-019 - Formatting Tooling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Formatting, code style, editorconfig, dotnet-format, analyzers, developer experience

## Context

### Problem Statement
The project needs a consistent and enforceable approach to code formatting and style. Without a standard, formatting differences can lead to noisy pull requests, inconsistent code style, and long-term maintenance issues.

### Required Outcomes
- Ensure consistent formatting and style across the codebase.
- Minimize formatting-related churn in pull requests.
- Use supported and modern tooling aligned with the .NET ecosystem.
- Shift formatting feedback earlier in the development process.

### Options
1. **No Formatting Tooling**
   - **Description:** Rely on developer discipline and manual formatting.
   - **Pros:** No tooling or setup required.
   - **Cons:** Inconsistent code style; formatting-only PR comments; long-term maintenance headache.
   - **Considerations:** Not sustainable for a team-based project.

2. **StyleCop**
   - **Description:** Use StyleCop analyzers to enforce formatting and style rules.
   - **Pros:** Historically popular in .NET projects; provides detailed rules.
   - **Cons:** No longer actively supported as a standalone solution; overlapping functionality with newer tooling; adds complexity.
   - **Considerations:** Largely superseded by EditorConfig and Roslyn analyzers.

3. **EditorConfig + dotnet-format**
   - **Description:** Define formatting and style rules in `.editorconfig` and enforce them using the `dotnet-format` CLI tool.
   - **Pros:** Actively supported; built into the .NET ecosystem; editor-agnostic; enforces whitespace, formatting, and analyzer rules consistently.
   - **Cons:** Requires tooling setup and CI integration.
   - **Considerations:** Works seamlessly with modern IDEs and CI pipelines; avoids vendor- or IDE-specific solutions.

## Impact
This decision affects all developers contributing to the codebase. Consistent formatting reduces PR noise, improves readability, and makes code reviews focus on logic rather than style.

## Related ADRs
ADR-012 (Deployments)  
ADR-013 (SCM Workflow)

## Security
No direct security implications.

## Recommendation
We recommend using **EditorConfig** to define formatting and style rules and enforcing them with **dotnet-format**. This approach provides a modern, supported, and maintainable solution aligned with the .NET ecosystem.

Formatting rules will be enforced:
- In CI pipelines (e.g., on pull requests).
- Optionally via local tooling such as Git hooks to catch issues earlier and reduce PR feedback cycles.

## Consequences
- Developers will receive immediate feedback when formatting rules are violated.
- Pull requests will contain fewer formatting-only changes.
- CI pipelines may fail if formatting rules are not followed.
- Slight upfront setup cost is offset by long-term reduction in friction and maintenance overhead.

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\]
 