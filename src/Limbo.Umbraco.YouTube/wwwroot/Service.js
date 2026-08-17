import { YouTubeAuth } from "@limbo/youtube/auth";

const baseUrl = "/umbraco/management/api/v1/limbo/youtube";

// Wraps "fetch" so requests are authorized against the Management API, and so the response body is parsed
// according to its content type. Rejects with the response for any non-2xx status code.
async function request(url, config) {

    config ??= {};
    config.method ??= "GET";
    config.headers ??= {};

    const token = await YouTubeAuth.TOKEN();
    config.headers.Authorization = `Bearer ${token}`;

    const res = await fetch(url, config);

    const contentType = res.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
        res.data = await res.json();
    } else if (contentType.startsWith("text/")) {
        res.textContent = await res.text();
    }

    if (!res.ok) throw res;

    return res;

}

async function get(url) {
    return await request(url);
}

export class YouTubeService {

    // Returns the server variables (version and cache buster) for this package
    static async getServerVariables() {
        const res = await get(`${baseUrl}/serverVariables`);
        return res.data;
    }

    // Looks up the video matching the specified source (URL or embed code)
    static async getVideo(source) {
        const res = await get(`${baseUrl}/video?source=${encodeURIComponent(source)}`);
        return res.data;
    }

    // Returns the largest thumbnail not wider than "maxWidth" - falls back to the smallest available
    static getThumbnail(video, maxWidth) {
        const thumbnails = video?.snippet?.thumbnails;
        if (!thumbnails) return null;

        const candidates = ["default", "medium", "high", "standard", "maxres"]
            .map((alias) => thumbnails[alias])
            .filter((x) => x && x.url)
            .sort((a, b) => (a.width ?? 0) - (b.width ?? 0));

        if (candidates.length === 0) return null;

        const withinBounds = candidates.filter((x) => (x.width ?? 0) <= (maxWidth ?? Number.MAX_SAFE_INTEGER));

        return withinBounds.at(-1) ?? candidates[0];
    }

}

export default YouTubeService;
