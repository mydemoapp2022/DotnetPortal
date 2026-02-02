# Responsive Design Guide

This guide outlines the responsive design standards for the BlazorTestGround application. All new pages and components should follow these guidelines to ensure consistent behavior across devices.

## Breakpoints

| Breakpoint | Max Width | Target Devices |
|------------|-----------|----------------|
| Mobile | 575px | Phones |
| Tablet | 1023px | Tablets, small laptops |
| Desktop | 1024px+ | Desktops, large screens |

### Media Query Usage

```css
/* Mobile styles (up to 575px) */
@media (max-width: 575px) {
    /* Mobile-specific styles */
}

/* Tablet styles (576px to 1023px) */
@media (max-width: 1023px) {
    /* Tablet and mobile styles */
}

/* Desktop styles (1024px and up) */
@media (min-width: 1024px) {
    /* Desktop-specific styles */
}
```

## CSS Organization: Global vs Component Styles

**Not every component needs its own `@media` queries.** Follow this guide to determine where responsive styles belong.

### Where to Put Responsive Styles

| Location | What Belongs There |
|----------|-------------------|
| `app.css` (Global) | Utility classes, shared patterns, global element adjustments |
| Component `.razor.css` | Component-specific layout changes, internal element behaviors |

### Global CSS (`app.css`)

Put styles here that are **reusable across multiple components**:

- Visibility utilities (`.hide-mobile`, `.show-tablet`)
- Layout utilities (`.flex-col-mobile`, `.w-full-tablet`)
- Global element adjustments (`.page`, `.container-responsive`)
- Shared font size scaling
- Common patterns used site-wide

```css
/* Example: Global utility in app.css */
@media (max-width: 575px) {
    .flex-col-mobile { flex-direction: column; }
    .w-full-mobile { width: 100% !important; }
}
```

### Component CSS (`.razor.css`)

Put styles here that are **specific to that component's internal layout**:

- Component-specific layout changes (e.g., table pagination stacking)
- Internal element sizing that only applies to this component
- Behaviors unique to that component (e.g., horizontal scroll for data tables)

```css
/* Example: Component-specific in LandingPageTable.razor.css */
@media (max-width: 1023px) {
    .table-container {
        overflow-x: auto;  /* Only this table needs horizontal scroll */
    }
}

@media (max-width: 575px) {
    .pagination-container {
        flex-direction: column;  /* Only this pagination stacks */
    }
}
```

### When You DON'T Need Component Media Queries

If a component only needs simple responsive behavior (like stacking elements on mobile), **use utility classes instead of writing new media queries**:

```html
<!-- No component CSS needed - just use utility classes -->
<div class="flex-row flex-col-mobile gap-16">
    <div class="w-full-mobile">Column 1</div>
    <div class="w-full-mobile">Column 2</div>
</div>
```

### Decision Flowchart

```
Need responsive behavior?
    │
    ├─► Can it be achieved with existing utility classes?
    │       │
    │       ├─► YES → Use utility classes, no new CSS needed
    │       │
    │       └─► NO → Is this behavior reusable across components?
    │                   │
    │                   ├─► YES → Add utility class to app.css
    │                   │
    │                   └─► NO → Add @media query to component CSS
```

### Examples

**Use utility classes (no component CSS):**
```razor
<!-- Cards that stack on mobile -->
<div class="flex-row flex-col-mobile gap-16">
    <div class="card">Card 1</div>
    <div class="card">Card 2</div>
</div>
```

**Use component CSS (specific behavior):**
```css
/* DataGrid.razor.css - only this grid needs card view on mobile */
@media (max-width: 575px) {
    .data-grid {
        display: block;  /* Switch from table to card layout */
    }
    .data-row {
        border: 1px solid var(--color-gray-light);
        margin-bottom: var(--spacing-sm);
    }
}
```

## CSS Variables

Use these variables from `wwwroot/app.css` for consistent styling:

### Layout
```css
--width-page-max: 1440px;    /* Maximum page width */
--width-content: 920px;       /* Content area width */
--width-sidebar: 354px;       /* Sidebar width (desktop) */
--width-sidebar-collapsed: 56px;
```

### Spacing
```css
--spacing-xs: 4px;
--spacing-sm: 8px;
--spacing-md: 16px;
--spacing-lg: 24px;
--spacing-xl: 48px;
```

### Typography
```css
--font-size-xl: 32.44px;
--font-size-title: 20px;
--font-size-lg: 18px;
--font-size-md: 16px;
--font-size-sm: 14px;
--font-size-xs: 12px;
```

## Utility Classes

### Visibility Classes
```css
.hide-mobile    /* Hidden on mobile */
.hide-tablet    /* Hidden on tablet */
.hide-desktop   /* Hidden on desktop */
.show-mobile    /* Visible only on mobile */
.show-tablet    /* Visible only on tablet */
.show-desktop   /* Visible only on desktop */
.hide-sm-down   /* Hidden on mobile + small tablet (< 768px) */
.hide-md-down   /* Hidden on mobile + tablet (< 1024px) */
```

### Layout Classes
```css
.flex-col-mobile    /* Column direction on mobile */
.flex-col-tablet    /* Column direction on tablet */
.w-full-mobile      /* Full width on mobile */
.w-full-tablet      /* Full width on tablet */
.text-center-mobile /* Center text on mobile */
```

## Component Guidelines

### 1. Containers
- Use `width: 100%` with `max-width` instead of fixed widths
- Apply `box-sizing: border-box` to include padding in width calculations

```css
/* Good */
.container {
    width: 100%;
    max-width: 1200px;
    box-sizing: border-box;
}

/* Avoid */
.container {
    width: 1200px;
}
```

### 2. Tables
- Wrap tables in a container with `overflow-x: auto` for mobile/tablet only
- Use `table-layout: fixed` with percentage or flexible widths when possible

```css
/* Table container - enable scroll only on smaller screens */
.table-container {
    width: 100%;
    max-width: 1392px;
}

@media (max-width: 1023px) {
    .table-container {
        overflow-x: auto;
        -webkit-overflow-scrolling: touch;
    }
}
```

### 3. Grids
- Use CSS Grid with responsive column counts
- Switch to single column on mobile

```css
.grid-view {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: var(--spacing-md);
}

@media (max-width: 1023px) {
    .grid-view {
        grid-template-columns: 1fr;
    }
}
```

### 4. Flexbox Layouts
- Allow wrapping with `flex-wrap: wrap`
- Change direction on mobile with `flex-direction: column`

```css
.flex-container {
    display: flex;
    gap: var(--spacing-md);
}

@media (max-width: 575px) {
    .flex-container {
        flex-direction: column;
    }
}
```

### 5. Images
- Use `max-width: 100%` to prevent overflow
- Consider different image sizes for different breakpoints

```css
img {
    max-width: 100%;
    height: auto;
}
```

### 6. Typography
- Reduce font sizes on mobile for better readability
- Use relative units or CSS variables

```css
.title {
    font-size: var(--font-size-xl);
}

@media (max-width: 575px) {
    .title {
        font-size: var(--font-size-title);
    }
}
```

### 7. Spacing
- Reduce padding and margins on mobile
- Use CSS variables for consistency

```css
.card {
    padding: var(--spacing-lg);
}

@media (max-width: 575px) {
    .card {
        padding: var(--spacing-md);
    }
}
```

## Component-Specific Responsive Widths

### Sidebar (SideMenu)
| Breakpoint | Width |
|------------|-------|
| Desktop | 354px |
| Tablet | 280px |
| Mobile | 220px |
| Minimized (all) | 56px (desktop/tablet), 48px (mobile) |

### Header Heights
| Breakpoint | Top Bar | Nav Bar |
|------------|---------|---------|
| Desktop | 123px | 66px |
| Tablet | 80px | 56px |
| Mobile | 60px | 48px |

## Best Practices

### Do's
1. **Mobile-first approach**: Start with mobile styles, then add complexity for larger screens
2. **Use CSS variables**: Maintain consistency with predefined spacing and sizing
3. **Test all breakpoints**: Verify layouts at 375px, 768px, and 1024px+ widths
4. **Use flexible units**: Prefer `%`, `fr`, `rem`, `em` over fixed `px` values
5. **Consider touch targets**: Minimum 44px for clickable elements on mobile
6. **Use scoped CSS**: Place styles in `.razor.css` files for component isolation

### Don'ts
1. **Avoid fixed widths**: Don't use pixel widths that exceed mobile viewport
2. **Don't hide critical content**: Important functionality should be accessible on all devices
3. **Avoid horizontal scroll**: Unless for data tables, prevent horizontal overflow
4. **Don't use `!important`**: Unless overriding Bootstrap or third-party styles
5. **Avoid inline styles**: Use CSS classes for maintainability

## File Structure

```
Features/
├── Shared/Layout/
│   ├── Header.razor
│   ├── Header.razor.css          ← Responsive header styles
│   ├── SideMenu.razor
│   ├── SideMenu.razor.css        ← Responsive sidebar styles
│   ├── MainLayout.razor
│   ├── MainLayout.razor.css      ← Responsive layout styles
│   ├── Footer.razor
│   └── Footer.razor.css          ← Responsive footer styles
├── Feature-Name/Pages/
│   └── [PageName]/
│       ├── [PageName].razor
│       └── [PageName].razor.css  ← Page-specific responsive styles
wwwroot/
└── app.css                       ← Global styles and utility classes
```

## Testing Checklist

Before submitting a new page or component, verify:

- [ ] Layout works at 375px width (mobile)
- [ ] Layout works at 768px width (tablet)
- [ ] Layout works at 1024px width (small desktop)
- [ ] Layout works at 1440px width (large desktop)
- [ ] No horizontal scrollbar on any breakpoint (unless intentional for tables)
- [ ] Text is readable at all sizes
- [ ] Touch targets are at least 44px on mobile
- [ ] Images don't overflow their containers
- [ ] Forms are usable on mobile
- [ ] Navigation is accessible on all devices
