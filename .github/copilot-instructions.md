When creating a commit message, please follow these guidelines:
Use those key word between square brackets to indicate the type of change you are making. For example, if you are fixing a bug, use [Fix]. If you are adding a new feature, use [Feature]. If you are updating documentation, use [Docs]. You can also use [Refactor] for code changes that do not add new features or fix bugs, and [Test] for changes related to testing. Or even [Chore] for changes that do not modify src or test files.

While including a description of the change, please keep it concise and clear. Use the imperative mood in the subject line (e.g., "Add feature" instead of "Added feature" or "Adding feature"). If your commit relates to an issue, include the issue number in the description (e.g., "Fixes #123").

When adding file path inside a typescript file, always use the absolute path starting from the root of the project. For example, if you are importing a file located in the src folder, use the following syntax: `import { MyComponent } from 'src/components/MyComponent';`. This will help avoid issues with relative paths and make it easier to navigate the project structure.
