---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: SCM Workflow"
labels: 'ADR'
---

# ADR-013 - SCM Workflow

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
SCM, Git, workflow, branching strategy, OneFlow, GitFlow, trunk-based, GitHub Flow

## Context

### Problem Statement
We need to define a **source control workflow** that the team will use for branches, merges, releases, and tagging. The workflow should support consistent releases, minimal long-lived branching complexity, and clear rules for feature integration and hotfixing.

### Required Outcomes
- Avoid maintaining multiple long‑lived branches.  
- Provide a clear trigger for releases.  
- Support tagging for versioning and hotfix workflows.  
- Align source control process with automated CI/CD practices.

### Options
1. **GitFlow**
   - **Description:** A traditional, structured Git workflow with multiple long‑lived branches such as `develop`, `main`, `feature`, `release`, and `hotfix`. It clearly defines how development and releases proceed but requires managing several active branches.
   - **Pros:** Well‑understood in many teams; supports multiple stages of development and parallel work.  
   - **Cons:** Multiple long‑lived branches; increased complexity in merging and maintenance.  
   - **Considerations:** Historically common but less ideal with modern automated delivery and a desire to reduce long‑lived branch overhead.

2. **Trunk‑Based / GitHub Flow**
   - **Description:** Developers integrate changes frequently into a single trunk (usually `main`). Feature branches (if used) are short‑lived and merged back quickly; `main` reflects production‑ready code or near‑ready code.
   - **Pros:** Simplifies branching; encourages frequent integration; pairs well with CI/CD.  
   - **Cons:** Without tagging or versioning conventions, may lack clear release boundaries.

3. **OneFlow**
   - **Description:** A Git workflow that uses a single long‑lived branch (e.g., `main`) as the basis for all development. Feature, release, and hotfix branches are short‑lived and merged back into `main`. Releases are triggered by **tags** on `main`, and tagging becomes the mechanism that drives the release process rather than merges or other branching structures.
   - **Pros:** Keeps repository history cleaner; minimizes long‑lived branches; tags provide a clear release trigger; easy to reason about in CI/CD processes.  
   - **Cons:** Requires discipline to manage feature visibility and ensure `main` stays production‑ready; feature flags or similar techniques may be needed for incomplete work.

## Impact
All developers and release engineers must align with the selected Git workflow. The CI/CD pipelines and deployment automation will use tags on `main` to drive releases rather than merges into multiple long‑lived branches.

## Related ADRs
  

## Security
No direct security implications beyond enforcing branch protections and access policies in the Git hosting platform.

## Recommendation
We recommend using **OneFlow** as the SCM workflow. This strategy uses **a single long‑lived branch (`main`)** and relies on **tags to trigger releases** rather than commit history or long‑lived development branches. This minimizes complexity, simplifies history, and supports automated delivery without maintaining multiple persistent branches.

## Consequences
- Developers will create feature/hotfix branches off `main` and merge back when ready.  
- Releases will be signaled by tagging `main` rather than by merging to a special branch.  
- Long‑lived development branches (e.g., `develop`) will not be used.  
- Ensure GitHub Actions workflows and deployment tooling are configured to react to tag events.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\]
 