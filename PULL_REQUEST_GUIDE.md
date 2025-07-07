# Pull Request Creation Guide

This guide provides instructions for creating individual pull requests for each bug fix found in the Squeal codebase.

## Overview

5 separate branches have been created, each containing a single bug fix. Each branch is ready to be pushed and turned into a pull request.

## Branch Summary

### 1. fix/azure-ad-error-handling
**Branch:** `fix/azure-ad-error-handling`  
**Commit:** `9f1262a`  
**File:** `Squeal/ConnectionFactory.cs`

**Description:** Adds proper error handling for Azure AD token acquisition to prevent unclear error messages and potential resource management issues.

**PR Title:** `Fix: Add proper error handling for Azure AD token acquisition`

**PR Description:**
```
This PR fixes error handling in the Azure AD authentication flow.

### Changes
- Added try-catch block around token acquisition
- Provides descriptive error messages when authentication fails
- Prevents potential connection disposal issues

### Impact
- Improved error reporting for authentication failures
- Better user experience with clear error messages
- Prevents runtime exceptions during token acquisition
```

### 2. fix/parameter-parsing-vulnerability
**Branch:** `fix/parameter-parsing-vulnerability`  
**Commit:** `58bebf9`  
**File:** `Squeal/RunCommand.cs`

**Description:** Fixes parameter parsing vulnerability that could cause IndexOutOfRangeException.

**PR Title:** `Fix: Improve parameter parsing in RunCommand`

**PR Description:**
```
This PR fixes a vulnerability in parameter parsing that could cause runtime exceptions.

### Changes
- Replaced unsafe Split('=') with IndexOf('=') for parameter parsing
- Added validation for parameter format (PARAM=VALUE)
- Added directory existence check for scripts directory
- Proper handling of edge cases (missing =, empty parameter names)

### Impact
- Prevents IndexOutOfRangeException from malformed parameters
- Better user feedback for invalid parameter formats
- Improved robustness of script execution
```

### 3. fix/option-description-typo
**Branch:** `fix/option-description-typo`  
**Commit:** `8026e83`  
**File:** `Squeal/Squeal.cs`

**Description:** Corrects the description for the --aad-client-secret option.

**PR Title:** `Fix: Correct description for --aad-client-secret option`

**PR Description:**
```
This PR fixes a typo in the option description.

### Changes
- Changed description from "The Azure AD ClientId" to "The Azure AD Client Secret"
- Ensures accurate documentation and help text

### Impact
- Improved user experience with correct documentation
- Prevents confusion about the option's purpose
```

### 4. fix/migration-file-validation
**Branch:** `fix/migration-file-validation`  
**Commit:** `a04ea88`  
**File:** `Squeal/MigrationRepository.cs`

**Description:** Adds robust validation for migration file format to prevent runtime exceptions.

**PR Title:** `Fix: Add robust validation for migration file format`

**PR Description:**
```
This PR adds comprehensive validation for migration file formats to prevent runtime exceptions.

### Changes
- Added filename format validation (ID_Name.extension)
- Used TryParse for integer parsing instead of Parse
- Added proper error handling with descriptive messages
- Added directory existence check
- Wrapped Operation constructor calls in try-catch

### Impact
- Prevents IndexOutOfRangeException from malformed filenames
- Prevents FormatException from invalid IDs
- Clear error messages for invalid file formats
- Improved robustness of migration processing
```

### 5. fix/directory-existence-check
**Branch:** `fix/directory-existence-check`  
**Commit:** `57f3350`  
**File:** `Squeal/UpCommand.cs`

**Description:** Adds directory existence check for migrations directory.

**PR Title:** `Fix: Add directory existence check for migrations directory`

**PR Description:**
```
This PR adds a directory existence check to prevent exceptions when the migrations directory doesn't exist.

### Changes
- Added check for migrations directory existence
- Provides clear error message when directory is missing

### Impact
- Prevents exceptions from enumerating files in non-existent directories
- Better user feedback when migrations directory is missing
- Improved error handling and user experience
```

## Instructions to Create Pull Requests

1. **Push branches to your fork/repository:**
   ```bash
   git push origin fix/azure-ad-error-handling
   git push origin fix/parameter-parsing-vulnerability
   git push origin fix/option-description-typo
   git push origin fix/migration-file-validation
   git push origin fix/directory-existence-check
   ```

2. **Create Pull Requests:**
   - Navigate to your GitHub repository
   - For each branch, create a new pull request
   - Use the titles and descriptions provided above
   - Target the `master` branch
   - Add appropriate labels (e.g., `bug`, `fix`, `enhancement`)

3. **Review Checklist:**
   - [ ] Each PR contains only one logical fix
   - [ ] Code follows existing style and conventions
   - [ ] Changes are backward compatible
   - [ ] Error messages are clear and helpful
   - [ ] No breaking changes introduced

## Testing

Before merging, consider testing:
- Azure AD authentication with invalid credentials
- Parameter parsing with malformed inputs
- Migration processing with invalid file names
- Operations with missing directories
- Edge cases and error conditions

## Notes

- Each branch contains a single, focused fix
- All changes are backward compatible
- Error handling has been improved across the board
- User experience has been enhanced with better error messages
- No new dependencies were introduced