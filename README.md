# LsChanged (lschanged), 'List changed files'

LsChanged is a command-line utility for taking filesystem metadata
snapshots and reporting files that changed between snapshots.
It can identify added, modified, unmodified, and deleted files
without copying file contents or calculating content hashes.

## What it does

LsChanged records the _state_ of files in a directory at a point in time. 
A _snapshot_ contains each readable file's path, size, last-write time,
file attributes, and Unix file mode. Comparing two snapshots produces
a sorted list of paths whose recorded state changed.

A typical workflow is:

1. Scan a directory to create a baseline snapshot.
2. Change files in that directory.
3. Scan the directory again.
4. Compare the snapshots and write the results to a file or standard
   output.

## Basic workflow

The following examples use POSIX paths. On Windows, use absolute Windows
paths such as `C:\\Temp\\LsChangedStore` and `C:\\Work\\MyProject`.

```bash
# Record the initial state.
lschanged scan /work/my-project -s /work/.lschanged-store

# Record a later state after files have changed.
lschanged scan /work/my-project -s /work/.lschanged-store

# Report added, modified, and deleted paths.
lschanged compare lp amd /work/changed.txt \
  -rp /work/my-project -s /work/.lschanged-store

# Write the report to standard output instead.
lschanged compare lp amd - \
  -rp /work/my-project -s /work/.lschanged-store
```

The store directory is created automatically when needed. Every `scan`
adds a new snapshot; it does not update an existing snapshot.
The `-s` store option is required for
`scan`, `compare`, `list`, `delete`, and `clear`.

## Command reference

### `scan`

```text
lschanged scan <path> [-fs <mode>] -s <store-path> [-v]
```

Recursively scans `<path>` and adds a snapshot to the store. Unreadable
or missing files and directories are skipped.

The `-fs` option controls directory symbolic links:

| Mode | Behavior |
| --- | --- |
| `0` | Skip directory symlinks (default); regular directories are still traversed. |
| `1` | Follow directory symlinks while attempting to avoid revisiting a directory target. |
| `2` | Follow directory symlinks without recursion protection. |

LsChanged compares metadata only. A file whose contents changed but whose
size, timestamp, attributes, and mode did not change is considered
unmodified.

### `compare`

```text
lschanged compare <mode> <states> <output-file> [-rp <folder>] [-i <ignore-file>] -s <store-path> [-v]
```

Compares two snapshots and writes one path per line. Output is sorted
lexicographically before it is written.

Comparison modes:

| Mode | Meaning |
| --- | --- |
| `lp` | Compare the last snapshot with the previous snapshot. |
| `lf` | Compare the last snapshot with the first snapshot. |
| `n,m` | Compare snapshot ordinal `n` (new) with ordinal `m` (old). |

If the store contains one snapshot, `lp` and `lf` compare it with
an empty snapshot, so all files are treated as added. An empty store,
or a specified ordinal that does not exist, is an error.

The `<states>` argument selects which categories to include. Use
a compact value such as `amd` or a comma-separated value such as `a,m,d`:

| Code | State |
| --- | --- |
| `a` | Added: present only in the new snapshot. |
| `m` | Modified: present in both snapshots but metadata differs. |
| `u` | Unmodified: present in both snapshots with equal metadata. |
| `d` | Deleted: present only in the old snapshot. |

Use `-` as `<output-file>` to write to standard output. Other output
files are written as UTF-8 text. Errors are written to standard error.

The `-rp <folder>` option removes that exact, case-sensitive root
prefix from matching output paths. The folder does not need to exist
and is useful for producing paths relative to a project root.

### `list`

```text
lschanged list -s <store-path> [-v]
```

Lists stored snapshots and their zero-based ordinals. Use the current
list output when selecting an ordinal; snapshot order is based on
filesystem enumeration and should not be assumed from filenames alone.

### `delete`

```text
lschanged delete <ordinal-or-last> -s <store-path> [-v]
```

Deletes a snapshot by zero-based ordinal. Use `last` to delete
the last listed snapshot.

### `clear`

```text
lschanged clear -s <store-path> [-v]
```

Deletes all snapshot files while leaving the store marker in place.

### `newignore`

```text
lschanged newignore <ignore-file> [-v]
```

Creates a UTF-8 ignore-file template and refuses to overwrite
an existing file.

### `-v` verbose output

The `-v` option enables diagnostic output such as scan progress,
snapshot counts, comparison totals, and ignore-rule decisions.
Normal messages go to standard output; errors go to standard error.

## Ignore files

Ignore files affect only `compare` output. Create a template
with `newignore`, then pass it to `compare` with `-i`:

```bash
lschanged newignore /work/.lschangedignore
lschanged compare lp amd - \
  -rp /work/my-project -i /work/.lschangedignore \
  -s /work/.lschanged-store
```

Rules use this format:

```text
[options=<option> ]regex=<regular-expression>
```

Blank lines and lines whose first character is `#` are ignored.
A path is excluded when it matches any rule. Before matching,
the `-rp` root is removed when applicable; on Windows, backslashes
are converted to forward slashes.

Regular expressions use the syntax supported by the utility.
Matching is case-insensitive by default on Windows and
case-sensitive by default on Linux. Override the default
with `options=+i` or `options=-i`:

```text
# Ignore ZIP files.
regex=\.zip$

# Ignore a logs directory relative to -rp.
regex=^logs/

# Ignore bin and obj directories regardless of case.
options=+i regex=^(.*/)*(bin|obj)/
```

The ignore file template contains additional rule explanations
and examples.


## Exit codes

| Code | Meaning |
| ---: | --- |
| `0` | Success. |
| `1` | Comparison completed but no paths remained after filtering. |
| `2` | A snapshot was not found, or the store is empty for the requested operation. |
| `3` | The requested new ignore file already exists. |
| `4` | The ignore file does not exist. |
| `5` | The snapshot store cannot be accessed. |
| `6` | The ignore file contains invalid entries. |
| `253` | Invalid command-line syntax or argument. |
| `254` | Help was displayed. |
| `255` | Unexpected fatal error. |

## Developer documentation

### Build requirements

Development requires the .NET 10 SDK. The project targets `net10.0`.

Both Windows and Linux are supported. The build scripts produce trimmed,
self-contained, single-file binaries for:
* `win-x64`
* `win-arm64`
* `linux-x64`
* `linux-arm64`
* `linux-arm` (32-bit)

### Build from Windows

From the repository root, run:

```
cd build
build.cmd
```

### Build from Linux

From the repository root, run:
```
cd ./build
chmod +x ./build.sh
./build.sh
```
