# Commit Agent (tools/commit-agent.sh)

Simple helper to generate a conventional-style commit message from the current git changes and commit them.

Usage examples:

Stage all and commit:

```bash
tools/commit-agent.sh -a
```

Preview commit message without committing:

```bash
tools/commit-agent.sh --dry-run
```

Notes:
- The script uses simple heuristics (new files -> feat, modified -> fix, deleted -> chore).
- It infer a scope from file paths (e.g., `src/REU.App/*` -> `app`).
- Review the generated message before pushing.
