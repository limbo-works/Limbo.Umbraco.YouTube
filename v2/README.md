# V2






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







## Screenshots

![image](https://user-images.githubusercontent.com/3634580/191851451-b3521520-53b1-48fc-9770-0fab12df719d.png)  
*Insert video by URL*

![image](https://user-images.githubusercontent.com/3634580/191851581-52e346bc-b3a9-49b1-bd8b-cc31237f9812.png)  
*Insert video by embed code*






## Examples

This package features a **Limbo YouTube Video** property editor that allows users to insert a YouTube video from either it's URL or embed code. Properties that are using this property editor then exposes an instance of `YouTubeValue` (or `null` if the property is empty).

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
    YouTubeValue? video = media.Value<YouTubeValue>("video");

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