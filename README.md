# Limbo YouTube  [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) [![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.YouTube.svg)](https://www.nuget.org/packages/Limbo.Umbraco.YouTube) [![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.YouTube.svg)](https://www.nuget.org/packages/Limbo.Umbraco.YouTube) [![Our Umbraco](https://img.shields.io/badge/our-umbraco-%233544B1)](https://our.umbraco.com/packages/backoffice-extensions/limbo-youtube/)

This package features a property editor for inserting (via URL or embed code) a YouTube video. The property editor saves a bit of information about the video, which then will be availble in C#.

## Installation

Install for Umbraco 10 via [NuGet](https://www.nuget.org/packages/Limbo.Umbraco.YouTube/2.0.0-beta001):

```
dotnet add package Limbo.Umbraco.YouTube --version 2.0.0-beta001
```

Install for Umbraco 9 via [NuGet](https://www.nuget.org/packages/Limbo.Umbraco.YouTube/1.0.0-alpha005):

```
dotnet add package Limbo.Umbraco.YouTube --version 1.0.0-alpha005
```

## Configuration

In order to access the YouTube API, the package needs to be configured with a set of Google credentials, which should be added in your `appSettings.json` file like this:

```json
{
  "Limbo": {
    "YouTube": {
      "Credentials": [
        {
          "Key": "1f22f315-e208-4c5f-86c5-3793be89ba9c",
          "Name": "MyProject",
          "Description": "A description about the credentials.",
          "ApiKey": "Your server key here."
        }
      ]
    }
  }
}
```

**Key** should be a randomly generated GUID which will be used as a unique identifier for the credentials.

**Name** and **Description** are currently not used, but are meant to be shown in the UI to identify the credentials to the user.

**ApiKey** should be an API key obtained from the [Google Cloud Platform](https://console.cloud.google.com/) You can create new apps/projects via the [create project page](https://console.cloud.google.com/projectcreate). Once your project has been created, you can go to the [credentials page](https://console.cloud.google.com/apis/credentials) to create an API key.

The API key let's this package access the YouTube API on behalf of your app/project.
