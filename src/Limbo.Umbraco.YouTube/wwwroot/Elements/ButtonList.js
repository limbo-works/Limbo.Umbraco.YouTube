// Replaces the old AngularJS "ButtonList.html" configuration view. Items are matched on their "value" (rather
// than an alias) so the element can store nulls, booleans and strings alike - which is what the tri-state
// embed options (not specified / yes / no) and the cache level dropdown need.
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";

class LimboYouTubeButtonListElement extends UmbElementMixin(LitElement) {

    static properties = {
        value: {},
        readonly: { type: Boolean, reflect: true }
    };

    #items = [];

    set config(config) {
        if (!config) return;
        this.#items = config.getValueByAlias("items") || [];
        this.requestUpdate();
    }

    constructor() {
        super();
        this.value = null;
        this.readonly = false;
    }

    #isActive(item) {
        // Treat undefined and null as the same "not specified" value
        if (item.value === null || item.value === undefined) {
            return this.value === null || this.value === undefined;
        }
        return item.value === this.value;
    }

    #select(item) {
        if (this.readonly) return;
        this.value = item.value ?? null;
        this.dispatchEvent(new UmbChangeEvent());
        this.requestUpdate();
    }

    render() {
        return html`
            <div class="items">
                ${repeat(this.#items, (item) => String(item.value), (item) => html`
                    <uui-button
                        look=${this.#isActive(item) ? "primary" : "outline"}
                        color=${this.#isActive(item) ? "positive" : "default"}
                        title=${item.title ?? ""}
                        label=${item.label}
                        ?disabled=${this.readonly}
                        @click=${() => this.#select(item)}></uui-button>
                `)}
            </div>
        `;
    }

    static styles = css`
        :host {
            display: block;
        }

        div.items {
            display: flex;
            flex-wrap: wrap;
            gap: var(--uui-size-space-2);
        }
    `;

}

customElements.define("limbo-youtube-button-list", LimboYouTubeButtonListElement);

export { LimboYouTubeButtonListElement as element };
export default LimboYouTubeButtonListElement;
