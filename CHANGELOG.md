# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## 3.0.0

### Added

- Added support for the `IHttpClientBuilder` methods.

### Changed

- [BREAKING]: Removed the `innerHandler` parameter in the `RequestsSignatureDelegatingHandler` constructor; you must now use the `InnerHandler` property.

### Deprecated

### Removed

### Fixed

- Support for working with `IHttpClientFactory` ([Issue #2](https://github.com/nventive/RequestsSignature/issues/2)).
- Postman pre-request script not working when using urlencoded bodies.

### Security

## 2.0.0

### Added

- Added support for ASP.NET Core 3.0 (targeting `netcoreapp3.0`).

### Changed

### Deprecated

### Removed

### Fixed

### Security

## 1.0.0

### Added

- Initial version compatible with ASP.NET Core >= 2.1

### Changed

### Deprecated

### Removed

### Fixed

### Security

