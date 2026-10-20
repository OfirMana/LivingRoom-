# Verification

Recovery verification performed on Windows x64 on 20 October 2026.

| Check | Result |
| --- | --- |
| Five-project Debug build | Passed |
| Five-project Release build | Passed with portable .NET SDK 8.0.426 and net472 reference assemblies |
| Access 16.0 x64 connection and lookup | Passed |
| 23 integration checks on a disposable demo database | Passed on the final Release build, including apostrophes in names, passwords, task text, shopping items, and poll text |
| Google Calendar operations | Not run; no original credentials used |
| Visual desktop interaction | Not verified; desktop automation was unavailable |
| GitHub Actions clean Windows build | Passed: [run 39398436605](https://github.com/OfirMana/LivingRoom-/actions/runs/39398436605) |

On the continuation run, the Windows UI tool still failed during initialization
with `failed to write kernel assets: The system cannot find the path specified
(os error 3)`, including after a kernel reset. This blocks visual interaction and
real screenshots; the integration checks are not a substitute for those checks.

The integration test also exercises the generated desktop WCF client over a local
named pipe using the actual host and client configuration sections, including login
and nested family serialization. It does not certify every desktop
screen on another computer. The desktop host and client are configured to use
the same named-pipe transport exercised by the integration checks.
