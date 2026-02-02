# Automated Deployment Pipeline Requirements

## 1. Secret Management
- An acceptable secret store must be chosen to manage sensitive information (passwords, API keys, certificates).  
- Secrets **must not** be stored in encrypted app settings files or source control.  
- Secrets should be referenced securely in GitHub Actions workflows via GitHub Environments or the chosen secret store.

## 2. GitHub Repository & Branch Strategy
- `main` branch is the only protected branch.  
- CODEOWNERS for GitHub Actions workflows are **DevOps team members only**.  
- All changes to `main` must follow a pull request (PR) workflow.  

### PR Requirements
- PRs targeting `main` must pass the following automated checks before merging:
  - Unit tests  
  - Dependency checks  
  - SonarQube analysis  
- PRs require approval from a **Technical Lead or Architect** to be eligible for merge.

## 3. Build & Tag-Based Deployment
- Binaries are built only on **tag-based triggers**.  
- Tagging conventions:
  - Semantic versioning: `v1.12.3004`  
  - Optional labels: `v1.12.3004-hotfix_01_16_2026__12_40_00`  
- Only **Architects and Technical Leads** are authorized to tag the `main` branch.  
- The tag must be passed into the build process as a variable for the application to reference at runtime for:
  - Displaying the current deployed version inside the app  
  - Tracking which version is deployed in which environment  

## 4. Deployment Stages & Approvals
- Deployment occurs in multiple stages using the same build artifact (binaries):
  - **Dev Stage**: Automatically triggered by tag; only Arch and Tech Leads can initiate.  
  - **UAT Stage**: Requires approval from QA team.  
  - **Production Stage**: Requires approval from client users.  
- All deployments use **on-prem GitHub Actions runners**.  

### Deployment Checks
- Each deployment stage must include:
  - Environment health check  
  - Integration tests  

## 5. Workflow Summary
- Code merged to `main` → PR checks executed (unit tests, dependency checks, SonarQube) → Tech Lead/Architect approval.  
- `main` tagged → Build workflow triggered → Binaries created once → Sequential deployments to Dev → UAT → Production with required approvals.  

## 6. Artifacts
- All build artifacts (binaries) must be versioned and stored for reuse across environments.  
- Deployments do **not** rebuild the binaries; they only deploy the pre-built artifacts.

## 7. Environment Configuration & AppSettings Usage
- Application mode and environment-specific behavior are controlled **via GitHub workflow variables**, not stored in application settings.  
- `appsettings.json` and `appsettings.Development.json` files are used **only for controlling application mode**, not environment-specific configuration:
  - Indicate whether the app is running in **development mode** or **production mode**.  
  - Control features such as verbose logging, debug information, or development-only behaviors.  
  - Must **not** include secrets, connection strings, or other environment-specific configuration values.  
- Configuration elements that vary per environment must be stored externally and injected during deployment, following the "build once" principle.  
- Only mode-specific settings (logging level, feature flags) belong in `appsettings`.
 