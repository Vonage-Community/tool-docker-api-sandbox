# Getting Involved

Thanks for your interest in the project. We’d love to have you involved!  
Check out the sections below to learn how to get started.

## Opening an Issue

We always welcome issues. 
If you’ve encountered unexpected behavior or have a suggestion for a new feature, please open an issue.

Include as much context as possible (what happens, expected behavior, how to reproduce). 
This helps us triage and address it more effectively.

## Making a Code Change

We’re open to pull requests! To keep the review process smooth:

- Keep changes small and focused.
- Clearly describe what you’re changing and why.
- Aim to maintain or improve overall code quality.

If you're unsure about the direction of a change, feel free to open an issue first to discuss it.

When you're ready:

1. Fork the repository.
2. Create a new branch for your changes.
3. Make your changes and commit them.
4. Open a pull request with a clear explanation of your work.

---

## Test Requirements

All changes must be **backed by tests**.

We follow a **test-first approach**, and every feature or fix must be covered by a test in one of the following
locations:

- **`Test/Features`** for functional or behavioral coverage of features and bug fixes, using test-specifications.
- **`Test/Products`** for API contract coverage based on our OpenAPI specs (e.g., "Sending a WhatsApp message").

These tests ensure that:

- New functionality behaves as expected.
- Existing API behavior (for each product) remains stable over time.

> Pull requests that are not covered by tests will be flagged during review.

What's asserted in a test _now_ is guaranteed to work _later_.