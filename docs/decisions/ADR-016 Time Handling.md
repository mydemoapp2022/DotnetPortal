---
name: ADR Template
about: Architectural Decision Records
title: "[ADR]: Time Handling"
labels: 'ADR'
---

# ADR-016 - Time Handling

Date: 2026-01-22

## Primary Contact
Brian Havice

## Status
Draft

## Keywords
Time, date, timezone, datetime, consistency, local time, agency standard

## Context

### Problem Statement
The application needs to record and display dates and times consistently. Users may be in different timezones or regions, and the agency requires operations to align with their local standard. A consistent approach is required for recording, displaying, and interpreting date and time values across the application.

### Required Outcomes
- Ensure all recorded times are consistent and unambiguous.  
- Display dates and times correctly for users in various timezones.  
- Align with existing agency practices and databases.  
- Minimize developer confusion and reduce conversion errors.

### Options
1. **Store UTC Internally, Convert to Local for Display**
   - **Description:** Record all times in UTC and convert to local timezones for display.  
   - **Pros:** Industry standard; avoids ambiguity; simplifies storage across multiple locations.  
   - **Cons:** May differ from agency's current practice; requires conversion logic on read/write.  
   - **Considerations:** Often simplest for global applications, but can conflict with legacy systems.

2. **Store Local Agency Time with Offsets**
   - **Description:** Store time in the agency’s local timezone, optionally including an offset. Convert to user’s local timezone for display.  
   - **Pros:** Matches agency’s current standard; easier to reason about relative to business processes; conversions for display are straightforward.  
   - **Cons:** Slightly more complex if users are in multiple timezones; risk of ambiguity if offsets are not included.  
   - **Considerations:** Recommended if the agency already stores timestamps this way.

3. **Store Local Time Only (No UTC or Offset)**
   - **Description:** Record all times as local to the agency without offsets; convert for display using application logic or a standard service.  
   - **Pros:** Simplest to match agency’s current practice; no complex offsets or conversions for agency-centric operations.  
   - **Cons:** Ambiguity arises if multiple timezones are involved; requires careful conversion for browser display.  
   - **Considerations:** Can be mitigated with a standard service to consistently translate to user’s timezone.

4. **Hybrid / Service-Oriented Approach**
   - **Description:** Store time in the agency’s local timezone, and use a standardized in-house service to translate times to the user’s local browser timezone as needed. Avoid storing UTC internally.  
   - **Pros:** Matches agency storage; ensures consistent conversions; centralizes logic for timezone translation; simplifies application code.  
   - **Cons:** Requires maintaining the translation service; potential for errors if service is misused.  
   - **Considerations:** Provides consistent, maintainable, and agency-aligned handling of time across the application.

## Impact
This decision affects all features that record, process, or display time, including activity tracking, form submissions, reports, and logs. Developers must use the standard service for all conversions to ensure consistency.

## Related ADRs
ADR-009 (Activity Tracking)

## Security
No direct security implications, though proper logging and auditing require consistent timestamps.

## Recommendation
We recommend **storing timestamps in the agency’s local time and using a standardized service to convert to the user’s browser timezone as needed**. This approach aligns with current agency practices, avoids UTC-based storage, and centralizes conversion logic for maintainability.

## Consequences
- All developers must use the standard service to convert times for display.  
- Data stored in the database will match the agency’s local time standard.  
- Users in other timezones will see times correctly converted for their local context.  
- Future timezone changes (e.g., daylight savings adjustments) are handled consistently via the service.  

## Decision  
\[The DWD UI Employer Portal Leadership Team should complete this section following the decision, and include who made the decision, the date, and attendees.\] 
 