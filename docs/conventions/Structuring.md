# Structuring Convention

This document defines the folder structure conventions for our Blazor project, following a **Vertical Slice Architecture (VSA)** approach. The goal is to:

- Group by **feature**, keeping all related functionality together.
- Separate **Pages** and **Components** as logical UI buckets.
- Keep **feature-specific services, models, or logic** inside the feature folder.
- Organize **cross-feature shared functionality** in a dedicated `Shared` section.

## Folder Structure Overview

Example structure:
```
/Features
│
├── Feature1
│   ├── Pages
│   │   └── Feature1Page.razor
│   │
│   ├── Components
│   │   └── Feature1Widget.razor
│   │
│   └── Feature1Logic.cs
│
├── Feature2
│   ├── Pages
│   │   └── Feature2Page.razor
│   │
│   ├── Components
│   │   └── Feature2Widget.razor
│   │
│   └── Feature2Logic.cs
│
├── Feature3
│   ├── Pages
│   │   └── Feature3Page.razor
│   │
│   ├── Components
│   │   └── Feature3Widget.razor
│   │
│   └── Feature3Logic.cs
│
├── Shared
│   ├── Accounts
│   │   ├── Pages
│   │   │   └── AccountOverview.razor
│   │   ├── Components
│   │   │   └── AccountCard.razor
│   │   ├── AccountService.cs
│   │   └── AccountModel.cs
│   │
│   ├── Notifications
│   │   ├── Components
│   │   │   └── NotificationBanner.razor
│   │   └── NotificationService.cs
│   │
│   └── SharedUtils
│       └── Models
│           └── SharedEnums.cs
```

## Conventions

### 1. Vertical Slice by Feature

- Each feature folder contains all elements needed for that feature:
  - Pages (routable `.razor` files)
  - Components (reusable UI pieces)
  - Models, services, or logic unique to the feature
- Feature folders are **self-contained**, making development, testing, and removal easier.

### 2. Pages and Components Bucketing

- **Pages**: `.razor` files with `@page` directives.
- **Components**: reusable pieces of UI within a feature.

### 3. Supporting Files

- Models, services, or any feature-specific logic should live **inside the feature folder**, not in Shared.
- File naming should indicate purpose, e.g., `EmployerRegistrationService.cs`.

### 4. Shared Features

- Anything used across multiple features lives in the `Shared` folder.
- Shared features follow the **same structure** as regular features:
  - Pages (if any)
  - Components (if any)
  - Services / Models
- Examples: `Shared/Accounts`, `Shared/Notifications`, `Shared/SharedUtils`.

### 5. Key Benefits

- Encourages **true vertical slicing**.
- Reduces **cross-feature coupling**.
- Makes onboarding new developers and scaling the project easier.
- Keeps folder structure **shallow and readable** while preserving clear boundaries.