# Session Management Guide

This guide outlines the session management architecture for the BlazorTestGround application. All session-related operations must follow these guidelines to ensure consistency, security, and maintainability.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        Blazor Components                        │
│                   (Pages, Components, Layouts)                  │
│                                                                 │
│    ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐   │
│    │ LandingPage │  │  Dashboard  │  │  Other Components   │   │
│    └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘   │
│           │                │                     │              │
└───────────┼────────────────┼─────────────────────┼──────────────┘
            │                │                     │
            ▼                ▼                     ▼
┌─────────────────────────────────────────────────────────────────┐
│                       Orchestrators                             │
│              (Session Access Allowed Here Only)                 │
│                                                                 │
│    ┌──────────────────┐     ┌─────────────────────────┐        │
│    │LandingOrchestrator│     │ DashboardOrchestrator  │        │
│    └────────┬─────────┘     └───────────┬─────────────┘        │
│             │                           │                       │
└─────────────┼───────────────────────────┼───────────────────────┘
              │                           │
              ▼                           ▼
┌─────────────────────────────────────────────────────────────────┐
│                      ISessionManager                            │
│                                                                 │
│         GetAsync<T>()  |  SetAsync<T>()  |  ClearAsync<T>()    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
              │
              ├─────────────────────────────────────┐
              ▼                                     ▼
┌─────────────────────────────────┐  ┌─────────────────────────────┐
│   ProtectedSessionManager       │  │  DistributedSessionManager  │
│   (Browser Session Storage)     │  │  (SQL Server Storage)       │
│   (Encrypted & Isolated)        │  │  (Server-side & Scalable)   │
└─────────────────────────────────┘  └─────────────────────────────┘
```

## Storage Providers

The application supports two session storage providers that can be switched via configuration:

### 1. Protected Session Storage (Default)
- Uses browser's `sessionStorage` with encryption
- Data is stored client-side
- Automatically isolated per browser tab
- Suitable for single-server deployments
- No additional infrastructure required

### 2. Distributed Session Storage (SQL Server)
- Uses SQL Server distributed cache
- Data is stored server-side
- Suitable for multi-server/load-balanced deployments
- Requires SQL Server database setup
- Better for scenarios where client-side storage is not desired

### Switching Between Providers

Change the `Session:StorageType` setting in `appsettings.json`:

```json
{
  "Session": {
    "StorageType": "Protected"  // or "Distributed"
  },
  "ConnectionStrings": {
    "SessionDb": "Server=(localdb)\\MSSQLLocalDB;Database=BlazorTestGroundSession;Trusted_Connection=True;"
  }
}
```

| StorageType | Description |
|-------------|-------------|
| `Protected` | Browser-based encrypted storage (default) |
| `Distributed` | SQL Server distributed cache |

## Core Principles

### 1. Session Access is Limited to Orchestrators

**Pages and Components must NEVER access session directly.**

Session data should only be read or written through orchestrator classes. This ensures:
- Single responsibility principle
- Easier testing and mocking
- Centralized session logic
- Clear separation of concerns

```csharp
// CORRECT - Orchestrator accesses session
public class DashboardOrchestrator : IDashboardOrchestrator
{
    private readonly ISessionManager _sessionManager;

    public async Task<Account?> GetSelectedAccountAsync()
    {
        var selectedAccount = await _sessionManager.GetAsync<SelectedAccount>();
        return selectedAccount?.Account;
    }
}

// INCORRECT - Component accessing session directly
public partial class DashboardPage
{
    [Inject]
    private ISessionManager SessionManager { get; set; } // DON'T DO THIS
}
```

### 2. All Session Models Must Implement ISessionModel

Every class stored in session must implement the `ISessionModel` interface. This provides:
- Type safety through generic constraints
- Clear identification of session-storable types
- Compile-time validation

```csharp
// Session/Models/ISessionModel.cs
public interface ISessionModel
{
}

// Session/Models/SelectedAccount.cs
public class SelectedAccount : ISessionModel
{
    public Account? Account { get; set; }
}
```

### 3. Session Data is User-Isolated

The `SessionManager` uses `ProtectedSessionStorage` which ensures:
- Data is stored in browser's sessionStorage (per-tab isolation)
- Data is encrypted using ASP.NET Core Data Protection
- Each user only sees their own session data
- Data is cleared when the browser tab is closed

## Components

### ISessionManager Interface

```csharp
public interface ISessionManager
{
    Task<T?> GetAsync<T>() where T : ISessionModel;
    Task SetAsync<T>(T model) where T : ISessionModel;
    Task ClearAsync<T>() where T : ISessionModel;
}
```

### ProtectedSessionManager Implementation

Uses browser's ProtectedSessionStorage for client-side encrypted storage.

```csharp
public class ProtectedSessionManager : ISessionManager
{
    private readonly ProtectedSessionStorage _sessionStorage;

    public ProtectedSessionManager(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task<T?> GetAsync<T>() where T : ISessionModel
    {
        var result = await _sessionStorage.GetAsync<T>(typeof(T).Name);
        return result.Success ? result.Value : default;
    }

    public async Task SetAsync<T>(T model) where T : ISessionModel
    {
        await _sessionStorage.SetAsync(typeof(T).Name, model);
    }

    public async Task ClearAsync<T>() where T : ISessionModel
    {
        await _sessionStorage.DeleteAsync(typeof(T).Name);
    }
}
```

### DistributedSessionManager Implementation

Uses SQL Server distributed cache for server-side storage.

```csharp
public class DistributedSessionManager : ISessionManager
{
    private readonly IDistributedCache _cache;
    private readonly ISessionIdProvider _sessionIdProvider;

    public DistributedSessionManager(IDistributedCache cache, ISessionIdProvider sessionIdProvider)
    {
        _cache = cache;
        _sessionIdProvider = sessionIdProvider;
    }

    public async Task<T?> GetAsync<T>() where T : ISessionModel
    {
        var key = $"{_sessionIdProvider.GetSessionId()}:{typeof(T).Name}";
        var data = await _cache.GetStringAsync(key);
        return string.IsNullOrEmpty(data) ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(T model) where T : ISessionModel
    {
        var key = $"{_sessionIdProvider.GetSessionId()}:{typeof(T).Name}";
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(model), DefaultCacheOptions);
    }

    public async Task ClearAsync<T>() where T : ISessionModel
    {
        var key = $"{_sessionIdProvider.GetSessionId()}:{typeof(T).Name}";
        await _cache.RemoveAsync(key);
    }
}
```

### Service Registration

```csharp
// Program.cs - Using extension methods
var sessionStorageType = builder.Configuration.GetValue<SessionStorageType>("Session:StorageType", SessionStorageType.Protected);

if (sessionStorageType == SessionStorageType.Distributed)
{
    var connectionString = builder.Configuration.GetConnectionString("SessionDb");
    builder.Services.AddSqlServerSessionStorage(connectionString);
}

builder.Services.AddSessionManagement(sessionStorageType);
```

## Creating a New Session Model

### Step 1: Create the Model Class

Create a new class in `Session/Models/` that implements `ISessionModel`:

```csharp
// Session/Models/UserPreferences.cs
namespace BlazorTestGround.Session.Models;

public class UserPreferences : ISessionModel
{
    public string Theme { get; set; } = "light";
    public string Language { get; set; } = "en";
    public bool ShowNotifications { get; set; } = true;
}
```

### Step 2: Add Orchestrator Methods

Add methods to the appropriate orchestrator to manage the session data:

```csharp
// In the orchestrator interface
public interface ISettingsOrchestrator
{
    Task<UserPreferences> GetPreferencesAsync();
    Task SavePreferencesAsync(UserPreferences preferences);
}

// In the orchestrator implementation
public class SettingsOrchestrator : ISettingsOrchestrator
{
    private readonly ISessionManager _sessionManager;

    public SettingsOrchestrator(ISessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public async Task<UserPreferences> GetPreferencesAsync()
    {
        return await _sessionManager.GetAsync<UserPreferences>()
               ?? new UserPreferences();
    }

    public async Task SavePreferencesAsync(UserPreferences preferences)
    {
        await _sessionManager.SetAsync(preferences);
    }
}
```

### Step 3: Use in Components via Orchestrator

```csharp
// SettingsPage.razor.cs
public partial class SettingsPage
{
    [Inject]
    private ISettingsOrchestrator SettingsOrchestrator { get; set; } = default!;

    private UserPreferences _preferences = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _preferences = await SettingsOrchestrator.GetPreferencesAsync();
            StateHasChanged();
        }
    }

    private async Task SaveSettings()
    {
        await SettingsOrchestrator.SavePreferencesAsync(_preferences);
    }
}
```

## File Structure

```
Session/
├── ISessionManager.cs                ← Interface for session operations
├── ProtectedSessionManager.cs        ← Browser storage implementation (client-side)
├── DistributedSessionManager.cs      ← SQL Server storage implementation (server-side)
├── SessionIdProvider.cs              ← ISessionIdProvider interface & CircuitSessionIdProvider
├── SessionServiceExtensions.cs       ← DI registration extension methods
└── Models/
    ├── ISessionModel.cs              ← Marker interface for session models
    └── SelectedAccount.cs            ← Session model for selected account
```

| File | Purpose |
|------|---------|
| `ISessionManager.cs` | Defines the contract for session operations (Get, Set, Clear) |
| `ProtectedSessionManager.cs` | Implements session using browser's encrypted sessionStorage |
| `DistributedSessionManager.cs` | Implements session using SQL Server distributed cache |
| `SessionIdProvider.cs` | Provides unique session IDs for distributed storage |
| `SessionServiceExtensions.cs` | Extension methods for registering session services in DI |
| `Models/ISessionModel.cs` | Marker interface that all session models must implement |
| `Models/SelectedAccount.cs` | Stores the currently selected employer account |

## Best Practices

### Do's

1. **Create focused session models**: Each model should represent a single concept
   ```csharp
   // Good - Single purpose
   public class SelectedAccount : ISessionModel { }
   public class UserPreferences : ISessionModel { }

   // Avoid - Too broad
   public class AllSessionData : ISessionModel { }
   ```

2. **Use orchestrators for all session operations**: Keep session logic centralized

3. **Handle null gracefully**: Session data may not exist
   ```csharp
   var account = await _sessionManager.GetAsync<SelectedAccount>();
   return account?.Account; // Safe null handling
   ```

4. **Clear session data when appropriate**: Clean up when user logs out or navigates away
   ```csharp
   await _sessionManager.ClearAsync<SelectedAccount>();
   ```

5. **Access session in OnAfterRenderAsync**: ProtectedSessionStorage requires JavaScript interop
   ```csharp
   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
       if (firstRender)
       {
           _data = await Orchestrator.GetSessionDataAsync();
           StateHasChanged();
       }
   }
   ```

### Don'ts

1. **Don't inject ISessionManager in pages/components**: Always go through orchestrators

2. **Don't store sensitive data**: Avoid storing passwords, tokens, or PII directly

3. **Don't store large objects**: Session storage has size limits; keep models small

4. **Don't rely on session for critical data**: Session can be cleared; have fallback logic

5. **Don't access session during prerendering**: It will throw an exception
   ```csharp
   // This will fail during prerender
   protected override async Task OnInitializedAsync()
   {
       _data = await _sessionManager.GetAsync<MyModel>(); // DON'T
   }
   ```

## Testing

### Mocking ISessionManager

```csharp
public class MockSessionManager : ISessionManager
{
    private readonly Dictionary<string, object> _store = new();

    public Task<T?> GetAsync<T>() where T : ISessionModel
    {
        var key = typeof(T).Name;
        return Task.FromResult(_store.TryGetValue(key, out var value)
            ? (T?)value
            : default);
    }

    public Task SetAsync<T>(T model) where T : ISessionModel
    {
        _store[typeof(T).Name] = model!;
        return Task.CompletedTask;
    }

    public Task ClearAsync<T>() where T : ISessionModel
    {
        _store.Remove(typeof(T).Name);
        return Task.CompletedTask;
    }
}
```

### Testing Orchestrators

```csharp
[Fact]
public async Task GetSelectedAccountAsync_ReturnsAccount_WhenExists()
{
    // Arrange
    var mockSession = new MockSessionManager();
    var expectedAccount = new Account { LegalName = "Test Corp" };
    await mockSession.SetAsync(new SelectedAccount { Account = expectedAccount });

    var orchestrator = new DashboardOrchestrator(mockSession);

    // Act
    var result = await orchestrator.GetSelectedAccountAsync();

    // Assert
    Assert.Equal("Test Corp", result?.LegalName);
}
```

## Security Considerations

1. **Data Encryption**: ProtectedSessionStorage encrypts all data using Data Protection API
2. **Session Isolation**: Each browser tab has isolated session storage
3. **No Cross-User Access**: Users cannot access other users' session data
4. **Automatic Cleanup**: Data is cleared when the browser tab is closed
5. **HTTPS Required**: Data Protection works best over HTTPS in production
