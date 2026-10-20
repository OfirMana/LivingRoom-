# Source provenance

- Drive archive: `ofirManaFamily.rar`, uploaded 6 June 2025, file ID
  `18IP5VpD7NLOhxfTUPvy_X7fmdqAElKfS`.
- Final project book: 161-page PDF dated June 2025, Drive file ID
  `17_wyTK3Ufka9sAWVT4IECx3PARgcFV6f`.
- Discovery date: 20 October 2026. No June 2026 project version was found in the
  searched Drive results.
- The 160 non-generated files in the local June 2025 project and the Drive archive
  matched by SHA-256, except for `FamilyProjectOfir.accdb`. Both original database
  variants are preserved in local backups; neither is the sanitized template here.

## Recovery changes

- Replace a hardcoded personal database path with a portable default and an
  explicit environment variable override.
- Keep working data under `.local/` when using the launcher.
- Allow local service features to work without the original Google credentials.
- Display a clear calendar configuration message, and stop automatically making
  family calendars public.
- Use x64 for both desktop executables in Debug and Release.
- Avoid overwriting the output database on every build; make the optional Google
  JSON project entry conditional and the Visual Studio WCF tooling import optional.
- Remove logging of usernames/passwords from the service login method.
- Handle apostrophes in additional legacy text SQL builders used by the main flows.
- Add launch/build/test scripts and database-backed integration checks.
- Use a local WCF named pipe for the desktop client and host, with a readiness
  signal and host cleanup, so the app does not require an administrator HTTP reservation.
- Support the legacy WPF markup build under a portable Windows .NET SDK as well as
  full-framework MSBuild; the runtime target remains .NET Framework 4.7.2.

The demo database was prepared from a copy: all non-city tables were emptied,
then the Access database was compacted into a new file with DAO to discard deleted
records. Only the city lookup and schema remain. The full original project book
is not published because it includes personal identifying details.
