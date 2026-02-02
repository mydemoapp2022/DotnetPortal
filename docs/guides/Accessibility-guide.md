# Accessibility Documentation

This document outlines the accessibility features implemented in the BlazorTestGround application to ensure compliance with WCAG 2.1 Level A and AA standards.

## Table of Contents

- [Standards Compliance](#standards-compliance)
- [Keyboard Navigation](#keyboard-navigation)
- [Screen Reader Support](#screen-reader-support)
- [Visual Accessibility](#visual-accessibility)
- [Component Reference](#component-reference)
- [Testing Guidelines](#testing-guidelines)

---

## Standards Compliance

This application targets **WCAG 2.1 Level AA** compliance, addressing the following principles:

| Principle | Description |
|-----------|-------------|
| **Perceivable** | Information and UI components must be presentable in ways users can perceive |
| **Operable** | UI components and navigation must be operable via keyboard |
| **Understandable** | Information and UI operation must be understandable |
| **Robust** | Content must be robust enough for assistive technologies |

---

## Keyboard Navigation

### Skip Link

A "Skip to main content" link is provided at the top of every page. This allows keyboard users to bypass repetitive navigation and jump directly to the main content.

**Usage:**
1. Press `Tab` when the page loads
2. The skip link becomes visible
3. Press `Enter` to skip to main content

### Focus Management

All interactive elements have visible focus indicators:

- **Focus outline:** 2px solid blue (`#258cfb`)
- **Focus offset:** 2px from element edge
- **Focus-visible:** Enhanced focus styles only appear for keyboard navigation

### Interactive Elements

| Element | Keyboard Action | Result |
|---------|-----------------|--------|
| Menu toggle | `Enter` or `Space` | Expands/collapses menu |
| Menu items | `Enter` or `Space` | Opens submenu or navigates |
| Table headers | `Enter` or `Space` | Sorts column |
| Table rows | `Enter` or `Space` | Navigates to detail view |
| Toggle buttons | `Enter` or `Space` | Switches view mode |
| Pagination | `Enter` or `Space` | Changes page |

### Tab Order

The application follows a logical tab order:

1. Skip link (visible on focus)
2. Header navigation
3. Side menu (when present)
4. Main content area
5. Footer

---

## Screen Reader Support

### Landmarks and Regions

| Landmark | Element | Purpose |
|----------|---------|---------|
| Navigation | `<nav role="navigation">` | Main side menu |
| Main | `<main id="main-content">` | Primary content area |
| Alert | `role="alert"` | Error notifications |

### ARIA Attributes

#### Navigation Menu

```html
<nav role="navigation" aria-label="Main navigation">
  <button aria-expanded="true/false" aria-label="Expand/Hide menu">
  <button aria-expanded="true/false" aria-current="true/page">
  <a aria-current="page">
```

- `aria-expanded`: Indicates submenu state (expanded/collapsed)
- `aria-current="page"`: Indicates the current page in navigation
- `aria-current="true"`: Indicates active menu section

#### Data Tables

```html
<table aria-label="Employer accounts">
  <caption class="visually-hidden">Description of table content</caption>
  <th scope="col" aria-sort="ascending/descending/none">
  <tr role="button" aria-label="View [account name] details">
```

- `aria-label`: Provides table purpose
- `aria-sort`: Announces current sort state
- `scope="col"`: Associates header with column data
- `role="button"`: Indicates clickable rows

#### Toggle Buttons

```html
<div role="group" aria-label="View options">
  <button aria-pressed="true/false">List</button>
  <button aria-pressed="true/false">Grid</button>
</div>
```

- `aria-pressed`: Indicates toggle state
- `role="group"`: Groups related buttons

### Visually Hidden Content

Content that should be read by screen readers but not displayed visually uses the `.visually-hidden` class:

```css
.visually-hidden {
    position: absolute !important;
    width: 1px !important;
    height: 1px !important;
    padding: 0 !important;
    margin: -1px !important;
    overflow: hidden !important;
    clip: rect(0, 0, 0, 0) !important;
    white-space: nowrap !important;
    border: 0 !important;
}
```

**Used for:**
- Table captions
- Icon descriptions
- Form field instructions

---

## Visual Accessibility

### Color Contrast

All text meets WCAG AA contrast requirements:

| Element | Foreground | Background | Ratio |
|---------|------------|------------|-------|
| Body text | `#333333` | `#ffffff` | 12.6:1 |
| Links | `#006bb7` | `#ffffff` | 5.3:1 |
| Navigation | `#003663` | `#ffffff` | 12.1:1 |
| Error text | `#e50000` | `#ffffff` | 5.0:1 |

### Non-Color Indicators

Information is not conveyed by color alone:

#### Negative Balance Indicator

Negative balances display both:
- Red text color (`text-danger`)
- Visual indicator: Red circle with exclamation mark

```html
<td class="text-danger negative-balance">
    <span class="negative-indicator" aria-hidden="true">!</span>
    -$500.00
</td>
```

#### Sort State Indicators

Sort state is conveyed through:
- Icon change (ascending/descending arrows)
- `aria-sort` attribute for screen readers
- Alt text on sort icons

### Focus Visibility

Focus indicators are visible on all interactive elements:

```css
a:focus,
button:focus,
[tabindex]:focus {
    outline: 2px solid var(--color-blue-focus);
    outline-offset: 2px;
}
```

### Responsive Design

The application is accessible at all viewport sizes:
- Touch targets are minimum 44x44 pixels on mobile
- Text remains readable without horizontal scrolling
- Focus indicators remain visible at all sizes

---

## Component Reference

### SideMenu

| Feature | Implementation |
|---------|----------------|
| Semantic element | `<nav role="navigation">` |
| Menu toggle | `<button>` with `aria-expanded`, `aria-label` |
| Menu items with submenu | `<button>` with `aria-expanded` |
| Menu items without submenu | `<a>` element |
| Current page | `aria-current="page"` |
| Keyboard support | Full tab navigation, Enter/Space activation |

### LandingPageTable

| Feature | Implementation |
|---------|----------------|
| Table description | `aria-label` and `<caption>` |
| Column headers | `scope="col"` |
| Sort state | `aria-sort="ascending/descending"` |
| Sortable headers | `tabindex="0"`, keyboard handlers |
| Clickable rows | `role="button"`, `aria-label`, `tabindex="0"` |
| Negative values | Visual indicator + color |
| Form labels | `<label for="...">` on select |

### EmployerDashboardMain

| Feature | Implementation |
|---------|----------------|
| Toggle group | `role="group"`, `aria-label` |
| Toggle state | `aria-pressed="true/false"` |

### MainLayout

| Feature | Implementation |
|---------|----------------|
| Skip link | Hidden until focused |
| Main content | `id="main-content"` for skip link target |
| Error UI | `role="alert"`, accessible dismiss button |

---

## Testing Guidelines

### Keyboard Testing

1. **Tab navigation:** Verify all interactive elements are reachable
2. **Focus visibility:** Confirm focus indicator is visible on all elements
3. **Activation:** Test Enter and Space keys activate controls
4. **Skip link:** Verify skip link appears on first Tab press

### Screen Reader Testing

Test with at least one screen reader:

| Platform | Recommended Screen Reader |
|----------|--------------------------|
| Windows | NVDA (free) or JAWS |
| macOS | VoiceOver (built-in) |
| iOS | VoiceOver (built-in) |
| Android | TalkBack (built-in) |

**Test scenarios:**
1. Navigate through the side menu
2. Sort a table column
3. Click a table row
4. Use pagination controls
5. Switch between List and Grid views

### Automated Testing

Run automated accessibility checks:

```bash
# Using axe-core via browser extension
# Install "axe DevTools" browser extension
# Open DevTools > axe DevTools tab > Scan page

# Using Lighthouse
# Open DevTools > Lighthouse tab > Check "Accessibility" > Generate report
```

### Manual Checklist

- [ ] All images have appropriate alt text
- [ ] Form fields have associated labels
- [ ] Color is not the only means of conveying information
- [ ] Focus order is logical
- [ ] Focus indicators are visible
- [ ] Interactive elements are keyboard accessible
- [ ] ARIA attributes are used correctly
- [ ] Headings follow proper hierarchy
- [ ] Error messages are announced to screen readers

---

## Resources

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [WAI-ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)
- [Microsoft Accessibility Guidelines](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/providing-accessibility-information)
- [Blazor Accessibility](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/css-isolation)

---

## Changelog

| Date | Version | Changes |
|------|---------|---------|
| 2026-01-23 | 1.0 | Initial accessibility implementation |

