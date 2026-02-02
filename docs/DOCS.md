# Project Documentation

This folder contains all the information a developer needs to understand and contribute to the project without
hunting through Jira, SharePoint, or emails. Each subfolder focuses on a specific type of information. Additionally,
this limits the need for developers to maintain access to external knowledge management systems.

## Folder Overview

### **decisions/**

> Contains Architecture Decision Records (ADRs) that document key technical decisions. Each file explains the problem,
> options considered, the chosen solution, and the reasoning behind it. Use this folder to understand why the system is
> designed the way it is.

### **conventions/**

> Holds project-specific coding, naming, formatting, and workflow conventions. Following these conventions keeps the
> code consistent, readable, and maintainable.

### **standards/**

> Contains broader rules the project follows, such as organizational, industry, or regulatory standards (e.g., security,
> testing, accessibility). Standards are generally non-negotiable and guide how code is written and maintained.

### **guides/**

> Step-by-step instructions for developers, including environment setup, running the project, and submitting pull
> requests. This is the first place to start when onboarding or contributing.

### **terms/**

> Defines project-specific and domain-specific terms, abbreviations, and acronyms. Use this folder to quickly understand
> the language and concepts used throughout the project.

## Additional Context

### Standards

**Standards** are broader rules or requirements that often come from the organization, industry, or regulatory bodies.
Examples include security practices, testing coverage requirements, accessibility compliance, or mandatory logging of
sensitive actions. Standards are usually mandatory and less flexible than conventions because they ensure the project
meets essential quality, safety, or legal requirements. Deviating from standards introduces risk, so they are treated as
non-negotiable guidelines that all contributors must follow.

Standards are typically defined by the organization in collaboration with the software architect. The architect helps
clarify which areas are constrained by standards and which areas allow flexibility, guiding the team toward safe and
effective implementation.

### Conventions

**Conventions** are project-specific agreements about how things should be done within this codebase. They are often
stylistic or procedural choices, such as variable naming, file organization, commit message formats, or logging
patterns. Conventions exist to make the code more consistent, readable, and maintainable for everyone on the team. They
are generally flexible and can evolve over time as the project changes. Following conventions helps maintain a smooth
workflow and reduces cognitive load when reading or modifying code.

Conventions are usually agreed upon collaboratively between the team and the software architect. This ensures that the
codebase remains consistent while still allowing the team to implement standards effectively.

#### Example of overlap between standards and conventions:

- The **format or style of log messages**, timestamps, field names, or wording, is a convention that can evolve for
  readability, while the **existence of certain log messages** may be mandated by a standard.

 