# Bug Fixes Summary for Squeal

This document summarizes the 5 bugs found and fixed in the Squeal database migration tool.

## Overview

Squeal is a .NET tool for running SQL Server database migrations and scripts during deployments. The following bugs were identified and fixed:

## Bug Fixes

### 1. Azure AD Token Acquisition Error Handling
**File:** `Squeal/ConnectionFactory.cs`  
**Commit:** `1d9c88e`

**Issue:** The Azure AD token acquisition code lacked proper error handling, which could lead to unclear error messages and potential resource management issues.

**Fix:** Added try-catch block around token acquisition with descriptive error messages.

**Impact:** Improved error reporting and prevents potential connection disposal issues during authentication failures.

### 2. Parameter Parsing Vulnerability
**File:** `Squeal/RunCommand.cs`  
**Commit:** `f0b41c7`

**Issue:** The parameter parsing logic used `Split('=')` which could cause `IndexOutOfRangeException` if parameters didn't contain equals signs or had multiple equals signs.

**Fix:** 
- Replaced `Split('=')` with `IndexOf('=')` for safer parsing
- Added validation for parameter format (PARAM=VALUE)
- Added directory existence check for scripts directory
- Proper handling of edge cases

**Impact:** Prevents runtime exceptions and provides better user feedback for malformed parameters.

### 3. Incorrect Option Description
**File:** `Squeal/Squeal.cs`  
**Commit:** `b666537`

**Issue:** The `--aad-client-secret` option had an incorrect description stating "The Azure AD ClientId" instead of "The Azure AD Client Secret".

**Fix:** Updated the description to accurately reflect the option's purpose.

**Impact:** Improved user experience with correct documentation.

### 4. Migration File Format Validation
**File:** `Squeal/MigrationRepository.cs`  
**Commit:** `d9f6691`

**Issue:** The `Operation` constructor assumed migration files followed a specific naming convention without validation, leading to potential `IndexOutOfRangeException` or `FormatException`.

**Fix:**
- Added comprehensive filename format validation
- Used `TryParse` for integer parsing instead of `Parse`
- Added proper error handling with descriptive messages
- Added directory existence check

**Impact:** Prevents runtime exceptions from malformed migration files and provides clear error messages.

### 5. Missing Directory Existence Check
**File:** `Squeal/UpCommand.cs`  
**Commit:** `ca4173e`

**Issue:** The `UpCommand` didn't check if the migrations directory existed before attempting to process migrations.

**Fix:** Added directory existence check with appropriate error message.

**Impact:** Prevents exceptions when the migrations directory doesn't exist and provides clear user feedback.

## Technical Details

### Code Quality Improvements
- **Error Handling:** All fixes include proper error handling with descriptive messages
- **Input Validation:** Added validation for user inputs and file formats
- **Resource Management:** Improved resource management and exception safety
- **User Experience:** Better error messages and feedback

### Security Considerations
- **Parameter Injection:** Fixed potential parameter parsing vulnerabilities
- **Input Sanitization:** Added proper validation for file names and formats

## Testing Recommendations

To verify these fixes work correctly, consider testing:

1. **Azure AD Authentication:** Test with invalid credentials to ensure proper error messages
2. **Parameter Parsing:** Test with malformed parameters (`PARAM`, `=VALUE`, `PARAM=VALUE=EXTRA`)
3. **File Format Validation:** Test with incorrectly named migration files
4. **Directory Handling:** Test with missing migrations/scripts directories
5. **Edge Cases:** Test with empty parameters, special characters, etc.

## Conclusion

These fixes significantly improve the robustness and user experience of the Squeal tool by:
- Preventing runtime exceptions
- Providing clear error messages
- Improving input validation
- Enhancing error handling

All fixes maintain backward compatibility while improving the tool's reliability and usability.