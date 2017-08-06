PageUp Compliance Vendor
=====

Purpose
-----

Purpose of this sample application is to show how third-party compliance solution vendor is able to interact with PageUp WorkCompliance API.


Websites
-----

[Live Demo](https://integration.dc0.pageuppeople.com/compliance-vendor/)

[PageUp Developers Portal - Work Compliance API](https://developers.pageuppeople.com/Api/Onboarding/WorkCompliance)

Setup
-----

### Dependencies
- [.NET Core 1.0.5 SDK 1.0.4](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.0.5-download.md)
- [Docker](https://www.docker.com/products/docker#/)


Local Development
-----

### Build

#### Command Line
```bash
# From root directory
dotnet restore
dotnet build
```

#### Visual Studio
- Clean Solution
- Rebuild Solution

### Run
#### Visual Studio
- Set startup projects *PageUp.Compliance.Vendor* 
- Debug

#### Command Line
```bash
# From project directory
dotnet run
# browse to http://localhost:5000/compliance-vendor
```

Docker Development
-----

### Build

```bash
# From root directory
docker-compose build
```

### Run

```bash
# From root directory
docker-compose up -d
# browse to http://localhost:9050/compliance-vendor
```
