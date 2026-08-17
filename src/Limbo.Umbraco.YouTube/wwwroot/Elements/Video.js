// Replaces the old AngularJS pair of "Views/Video.html" + "Scripts/Controllers/Video.js".
//
// The value stored in the database is exactly what the Management API returns (an intermediary video value), so
// unlike the AngularJS version this element does not need to copy fields one by one - it stores the response as
// is. The raw YouTube API response is nested as an escaped JSON string in "details._data", because Umbraco
// would otherwise mangle the timestamps in it.
import { html, css, nothing, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";
import { UmbFormControlMixin } from "@umbraco-cms/backoffice/validation";

import "@limbo/video/elements/duration";
import { YouTubeService } from "@limbo/youtube/service";

const THUMBNAIL_MAX_WIDTH = 320;

class LimboYouTubeVideoElement extends UmbFormControlMixin(UmbLitElement, undefined) {

    static properties = {
        readonly: { type: Boolean, reflect: true },
        mandatory: { type: Boolean },
        mandatoryMessage: { type: String }
    };

    #value = null;

    // The parsed contents of "details._data" - ie. the video as returned by the YouTube API
    #video = null;

    #loading = false;
    #error = "";
    #sourceInput;
    #debounceTimer = 0;

    // Guards against an earlier, slower lookup overwriting the result of a later one
    #requestToken = 0;

    constructor() {
        super();
        this.readonly = false;
    }

    get value() {
        return this.#value;
    }

    set value(value) {
        const oldValue = this.#value;
        this.#value = value ?? null;
        this.#video = this.#parseDetails(this.#value);
        this.requestUpdate("value", oldValue);
    }

    connectedCallback() {
        super.connectedCallback();
        this.#video = this.#parseDetails(this.#value);
    }

    disconnectedCallback() {
        super.disconnectedCallback();
        window.clearTimeout(this.#debounceTimer);
    }

    firstUpdated() {
        this.#sourceInput = this.renderRoot.querySelector(".source");
        if (this.#sourceInput) this.addFormControlElement(this.#sourceInput);
    }

    // Reads the raw YouTube video out of the escaped JSON in "details._data". Values saved before 13.0.2 used a
    // "video" property instead of "details", so that shape is still supported.
    #parseDetails(value) {
        const details = value?.details ?? value?.video;
        if (!details?._data) return null;
        try {
            return JSON.parse(details._data);
        } catch {
            return null;
        }
    }

    #sourceValue() {
        return this.#value?.source ?? "";
    }

    #commit(value) {
        this.value = value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    #clear() {
        this.#error = "";
        this.#loading = false;
        window.clearTimeout(this.#debounceTimer);
        this.#requestToken++;
        this.#commit(null);
    }

    async #lookup(source) {

        const requestId = ++this.#requestToken;

        this.#loading = true;
        this.#error = "";
        this.requestUpdate();

        try {

            const data = await YouTubeService.getVideo(source);

            // A newer lookup has been started in the meantime, so discard this result
            if (requestId !== this.#requestToken) return;

            this.#loading = false;
            this.#commit(data);

        } catch (res) {

            if (requestId !== this.#requestToken) return;

            this.#loading = false;

            // The API returns a plain text message for the errors it knows about
            this.#error = res?.textContent
                || res?.data?.detail
                || this.localize.term("limboYouTube_genericError");

            // Keep what the user typed, but drop the stale video information
            this.#commit({ source });

        }

    }

    // [CHANGE: code review fix - a lookup that is already in flight has to be invalidated as soon as the user
    // types again, or its (older) response is still accepted by #lookup and overwrites the newer input. The
    // loading state is no longer set for the duration of the debounce either, because ".loading > div" sets
    // "pointer-events: none" and would make the whole editor unclickable while typing]
    // Related: Api/YouTubeSecurityFilter.cs, PropertyEditors/YouTubeVideoValueConverter.cs, documentation/UPGRADE-UMBRACO-17.md
    #scheduleLookup(source) {

        window.clearTimeout(this.#debounceTimer);

        // Discard the result of any lookup that is already in flight
        this.#requestToken++;

        const value = source.trim();
        if (!value) {
            this.#clear();
            return;
        }

        this.#error = "";
        this.requestUpdate();

        this.#debounceTimer = window.setTimeout(() => this.#lookup(value), 300);

    }

    #onSourceInput(event) {
        const source = event.target.value ?? "";
        this.#commit({ ...(this.#value ?? {}), source });
        this.#scheduleLookup(source);
    }

    #onRefresh() {
        // Cancel a debounced lookup, so refreshing doesn't fire a second, identical request
        window.clearTimeout(this.#debounceTimer);
        const source = this.#sourceValue().trim();
        if (!source) {
            this.#clear();
            return;
        }
        this.#lookup(source);
    }

    #renderEditor() {
        const source = this.#sourceValue();
        return html`
            <div class="editor">
                <h5>${this.localize.term("limboYouTube_urlOrEmbedCode")}</h5>
                <textarea
                    class="source"
                    rows="3"
                    .value=${source}
                    ?disabled=${this.readonly}
                    placeholder=${this.localize.term("limboYouTube_urlPlaceholder")}
                    @input=${this.#onSourceInput}></textarea>
                <div class="actions">
                    <uui-button
                        look="outline"
                        label=${this.localize.term("limboYouTube_refresh")}
                        ?disabled=${this.readonly || !source.trim()}
                        @click=${this.#onRefresh}></uui-button>
                    <uui-button
                        look="outline"
                        color="danger"
                        label=${this.localize.term("limboYouTube_clear")}
                        ?disabled=${this.readonly || !source.trim()}
                        @click=${this.#clear}></uui-button>
                </div>
            </div>
        `;
    }

    #renderError() {
        if (!this.#error) return nothing;
        return html`<p class="notice --error">${this.#error}</p>`;
    }

    #renderVideo() {

        const video = this.#video;
        if (!video) return nothing;

        const thumbnail = YouTubeService.getThumbnail(video, THUMBNAIL_MAX_WIDTH);
        const description = video.snippet?.description?.trim();

        return html`
            <div class="block">
                <h5>${this.localize.term("limboYouTube_video")}</h5>
                <div class="box">
                    <div class="card-row">
                        ${when(thumbnail, () => html`
                            <div class="thumbnail">
                                <img src=${thumbnail.url} alt=${video.snippet?.title ?? ""} loading="lazy" />
                            </div>
                        `)}
                        <table>
                            <tr>
                                <th>${this.localize.term("limboYouTube_id")}</th>
                                <td><code>${video.id}</code></td>
                            </tr>
                            <tr>
                                <th>${this.localize.term("limboYouTube_title")}</th>
                                <td>${video.snippet?.title}</td>
                            </tr>
                            ${when(video.contentDetails?.duration, () => html`
                                <tr>
                                    <th>${this.localize.term("limboYouTube_duration")}</th>
                                    <td><limbo-video-duration .value=${video.contentDetails.duration}></limbo-video-duration></td>
                                </tr>
                            `)}
                        </table>
                    </div>
                    ${when(description, () => html`<div class="description">${description}</div>`)}
                </div>
            </div>
        `;

    }

    render() {
        return html`
            <div class="shell ${this.#loading ? "loading" : ""}">
                <div>
                    ${this.#renderEditor()}
                    ${this.#renderError()}
                    ${this.#renderVideo()}
                </div>
                ${this.#loading ? html`<uui-loader></uui-loader>` : nothing}
            </div>
        `;
    }

    static styles = css`
        :host {
            display: block;
            position: relative;
        }

        .loading > div {
            opacity: 0.6;
            pointer-events: none;
        }

        .loading uui-loader {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
        }

        h5 {
            margin: 0;
        }

        .editor {
            display: grid;
            gap: var(--uui-size-space-3);
        }

        .source {
            width: 100%;
            resize: vertical;
            box-sizing: border-box;
            padding: var(--uui-size-space-3);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface);
            color: var(--uui-color-text);
            font: inherit;
        }

        .actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-3);
        }

        .notice {
            margin: var(--uui-size-space-3) 0 0;
            font-size: var(--uui-font-size-1);
        }

        .notice.--error {
            color: var(--uui-color-danger);
        }

        .block {
            margin-top: var(--uui-size-layout-1);
        }

        .box {
            padding: var(--uui-size-space-4);
            border: 1px solid var(--uui-color-border);
            border-radius: var(--uui-border-radius);
            background: var(--uui-color-surface-alt);
        }

        .card-row {
            display: flex;
            gap: var(--uui-size-space-4);
            align-items: flex-start;
            flex-wrap: wrap;
        }

        .thumbnail {
            flex: 0 0 270px;
            max-width: 270px;
            aspect-ratio: 16 / 9;
            border-radius: var(--uui-border-radius);
            overflow: hidden;
            background: var(--uui-color-surface);
        }

        .thumbnail img {
            width: 100%;
            height: 100%;
            object-fit: cover;
            display: block;
        }

        table {
            border-collapse: collapse;
        }

        th {
            text-align: left;
            padding: 2px var(--uui-size-space-4) 2px 0;
            color: var(--uui-color-text-alt);
            font-weight: 600;
            vertical-align: top;
            white-space: nowrap;
        }

        td {
            padding: 2px 0;
        }

        .description {
            margin-top: var(--uui-size-space-3);
            color: var(--uui-color-text-alt);
            white-space: pre-wrap;
        }
    `;

}

customElements.define("limbo-youtube-video", LimboYouTubeVideoElement);

export { LimboYouTubeVideoElement as element };
export default LimboYouTubeVideoElement;
