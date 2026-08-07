# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v2.0.0] - 2026-08-07

### Added

- Added the `Pelican.Testing` package with request dispatch, handler resolution, pipeline behavior tracing, replacement handlers, failed resolution assertions, and no-response request assertions.
- Added adapter-friendly testing contracts for external test host wrappers.
- Added CI and release workflows aligned with the OctoMap release structure.
- Added package source link and symbol package configuration.

### Changed

- Updated target frameworks to `net8.0`, `net9.0`, and `net10.0`.
- Updated package dependencies and moved package versions to central package management.
- Made `Pelican.Mediator.Void` public so no-response pipeline behaviors and testing assertions can use the same response marker.
- Updated README documentation for `Pelican.Mediator` and `Pelican.Testing`.

## [v1.1.2] - 2025-05-27

### Added

- Add support for Pre-Processors and Post-Processors.

## [v1.1.1] - 2025-05-24

### Added

- Add support for Publisher/Subscriber pattern.

### Changed

- Change lifetime to Transient for handlers.

## [v1.0.5] - 2025-05-23

### Added

- First stable release of Pelican.Mediator core.
