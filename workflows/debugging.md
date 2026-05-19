# Debugging Workflow

Use this workflow when something fails or behaves unexpectedly.

## Steps

1. Lead reproduces the issue or records why reproduction is not available.
2. Lead captures observed facts.
3. Lead lists competing hypotheses.
4. Lead assigns independent hypotheses to workers when useful.
5. Workers report evidence, not guesses.
6. Lead selects the smallest sufficient root-cause fix supported by evidence.
7. Lead verifies the original failure is resolved.
8. Lead adds or updates regression coverage when practical.

## Evidence Rules

Do not treat a plausible explanation as a fact. Facts come from command output, logs, source code, tests, documentation, or direct reproduction.

## Verification

The final report must include the reproduction command or manual steps and the passing verification after the fix.
