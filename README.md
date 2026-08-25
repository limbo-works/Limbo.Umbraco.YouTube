# Limbo YouTube

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/limbo-works/Limbo.Umbraco.YouTube/blob/v17/main/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/Limbo.Umbraco.YouTube.svg)](https://www.nuget.org/packages/Limbo.Umbraco.YouTube)
[![NuGet](https://img.shields.io/nuget/dt/Limbo.Umbraco.YouTube.svg)](https://www.nuget.org/packages/Limbo.Umbraco.YouTube)
[![Umbraco Marketplace](https://img.shields.io/badge/umbraco-marketplace-%233544B1)](https://marketplace.umbraco.com/package/limbo.umbraco.youtube)
[![Limbo.Umbraco.YouTube at packages.limbo.works](https://img.shields.io/badge/limbo-packages-blue)](https://packages.limbo.works/limbo.umbraco.youtube/)

**Limbo.Umbraco.YouTube** is a package for Umbraco that features a property editor for inserting (via URL or embed code) a YouTube video. The property editor saves a bit of information about the video, which then will be availble in C#.

The latest version (`v17.x`) supports Umbraco 17, whereas older releases support Umbraco 13 (`v13.x`), Umbraco 10-12 (`v2.x`) and Umbraco 9 (`v1.x`).

<table>
  <tr>
    <td><strong>License:</strong></td>
    <td><a href="https://github.com/limbo-works/Limbo.Umbraco.YouTube/blob/v17/main/LICENSE.md"><strong>MIT License</strong></a></td>
  </tr>
  <tr>
    <td><strong>Umbraco:</strong></td>
    <td>
      Umbraco 17
    </td>
  </tr>
  <tr>
    <td><strong>Target Framework:</strong></td>
    <td>
      .NET 10
    </td>
  </tr>
</table>








<br /><br />

## Installation

### Umbraco 17

The package targets Umbraco 17 and is available via [**NuGet**](https://www.nuget.org/packages/Limbo.Umbraco.YouTube/17.0.0-alpha001). To install the package, you can use either .NET CLI:

```
dotnet add package Limbo.Umbraco.YouTube --version 17.0.0-alpha001
```

or the NuGet package manager:

```
Install-Package Limbo.Umbraco.YouTube -Version 17.0.0-alpha001
```

This is a **prerelease** version. See [**documentation/UPGRADE-UMBRACO-17.md**](./documentation/UPGRADE-UMBRACO-17.md) for what changed when the package was ported from Umbraco 13 to Umbraco 17.

### Other versions of Umbraco

- [**`v13/main`**](https://github.com/limbo-works/Limbo.Umbraco.YouTube/tree/v13/main) Umbraco 13
- ~~[**`v2/main`**](https://github.com/limbo-works/Limbo.Umbraco.YouTube/tree/v2/main) Umbraco 10, 11 and 12~~ <sub title="Umbraco 10, 11 and 12 have reached end-of-life"><sup>(EOL)</sup></sub>
- ~~[**`v1/main`**](https://github.com/limbo-works/Limbo.Umbraco.YouTube/tree/v1/main) Umbraco 9~~ <sub title="Umbraco 9 has reached end-of-life"><sup>(EOL)</sup></sub>






<br /><br />

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







<br /><br />

## Screenshots

> **Note:** the screenshots below are from the Umbraco 13 version of the package. The property editor was
> rebuilt for the new Umbraco backoffice in `v17.x`, so it now looks different, although it works the same way.

![image](https://user-images.githubusercontent.com/3634580/191851451-b3521520-53b1-48fc-9770-0fab12df719d.png)  
*Insert video by URL*

![image](https://user-images.githubusercontent.com/3634580/191851581-52e346bc-b3a9-49b1-bd8b-cc31237f9812.png)  
*Insert video by embed code*







<br /><br />

## Examples

This package features a **Limbo YouTube Video** property editor that allows users to insert a YouTube video from either it's URL or embed code. Properties that are using this property editor then exposes an instance of `YouTubeVideoValue` (or `null` if the property is empty).

You can use the `YouTubeValue` instance like shown below:

```cshtml
@using Limbo.Umbraco.YouTube.Models.Videos

@inherits UmbracoViewPage

@{

    // Assuming video is created as a media, get a reference to that media
    IPublishedContent? media = Umbraco.Media(1178);

    if (media is null)
    {
        <pre>NOPE!</pre>
        return;
    }

    // Get the video value from the "video" property
    YouTubeVideoValue? video = media.Value<YouTubeVideoValue>("video");

    if (video is null)
    {
        <pre>NOPE!</pre>
        return;
    }

    // Ender the embed iframe
    @video.Embed.Html

    // Render the video ID
    <pre>@video.Details.Id</pre>

    // Render other video information
    <pre>@video.Details.Title</pre>
    <pre>@video.Details.Duration</pre>
    <pre>@video.Details.Description</pre>

    // Render largest available thumbnail
    YouTubeThumbnail? thumbnail = video.Details.Thumbnails.LastOrDefault();
    if (thumbnail is not null)
    {
        <img src="@thumbnail.Url" width="@thumbnail.Width" height="@thumbnail.Height" />
    }

}
```
