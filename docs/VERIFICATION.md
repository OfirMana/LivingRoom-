# Verification

Recovery verification performed on Windows x64 on 20 October 2026.

| Check | Result |
| --- | --- |
| Five-project Debug build | Passed |
| Five-project Release build | Passed with portable .NET SDK 8.0.426 and net472 reference assemblies |
| Access 16.0 x64 connection and lookup | Passed |
| 31 integration checks on a disposable demo database | Passed on the Release build, including linked updates, text quoting, isolated pending writes, and atomic rollback with no orphan login record |
| Google Calendar operations | Not run; no original credentials used |
| Visual desktop interaction | Not verified; desktop automation was unavailable |
| GitHub Actions clean Windows build | Passed: [run 39398436605](https://github.com/OfirMana/LivingRoom-/actions/runs/39398436605) |
| GitHub Actions build and downloadable packaging | Passed: [run 39399813465](https://github.com/OfirMana/LivingRoom-/actions/runs/39399813465) |
| Launcher and application startup | Host and client windows opened, remained responsive, and were closed after the startup check |

On the continuation run, the Windows UI tool still failed during initialization
with `failed to write kernel assets: The system cannot find the path specified
(os error 3)`, including after a kernel reset. This blocks visual interaction and
real screenshots; the integration checks are not a substitute for those checks.

The integration test also exercises the generated desktop WCF client over a local
named pipe using the actual host and client configuration sections, including login
and nested family serialization. It does not certify every desktop
screen on another computer. The desktop host and client are configured to use
the same named-pipe transport exercised by the integration checks.

The queue-isolation regression was run against the previous implementation and
failed as expected. It passed after replacing shared static change queues with
instance-owned queues. The rollback check deliberately throws during the second
stage of member insertion, verifies a zero result and restored ID, and directly
checks that the first-stage login row was rolled back.
