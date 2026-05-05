git# Commit Tool (tools/commit-tool.sh)

Simple helper to generate a conventional-style commit message from the current git changes and commit them.

Usage examples:

Stage all and commit:

```bash
tools/commit-tool.sh -a
```

Preview commit message without committing:

```bash
tools/commit-tool.sh --dry-run
```

Notes:
- The script uses simple heuristics (new files -> feat, modified -> fix, deleted -> chore).
- It infer a scope from file paths (e.g., `src/REU.App/*` -> `app`).
- Review the generated message before pushing.
