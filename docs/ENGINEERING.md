# Engineering notes

## Layered Windows application

LivingRoom separates its WPF client, WCF service contract, domain models, and
Access data access into five projects. The desktop host and client communicate
over a local named pipe. This preserves the client/server structure without an
HTTP URL reservation or an open network port.

## Parameterized database writes

Family, member, login-account, task, shopping, poll, option, and vote writes use
OLE DB parameters. OLE DB binds by position: values are added in the order of the
SQL placeholders. Text stays separate from SQL; dates, booleans, and integers have
explicit types. This avoids quoting errors and dependence on display date formats.
Legacy lookup and unused event/routine paths remain outside this targeted refactor.

## Atomic writes and request isolation

Each repository instance owns its pending changes. Saving one instance cannot
execute or clear another instance's queue.

`SaveChanges` uses one Access transaction. Member creation inserts a login account
and a linked member row: both commit or neither does. On failure, the transaction
rolls back, assigned in-memory IDs are restored, and zero saved rows is returned
to preserve the existing service/UI contract. Logs omit SQL values and credentials.

Regression tests deliberately fail the second half of an insert and query Access
directly to confirm there is no orphan login. They also interleave two repository
queues to verify isolation, using a disposable sanitized database copy.

## Reproducible delivery

GitHub Actions restores pinned dependencies, builds Release on Windows, and
packages the app with a SHA-256 checksum. Working databases, credentials, caches,
and private originals are excluded from Git. A clean template is copied to a
separate working database on first use.

## Scope and tradeoffs

Production use would require password hashing with a migration strategy,
service-side authorization, broader validation, and a reviewed calendar
authentication flow. Those changes affect persistent data or user behavior and
are not silently included in this refactor. Live Google operations and full
visual interaction remain outside the verification performed here.
