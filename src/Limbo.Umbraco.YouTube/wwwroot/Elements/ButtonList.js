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
                        class="${this.#isActive(item) ? "--active" : ""}"
                        look=${this.#isActive(item) ? "secondary" : "secondary"}
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

        uui-button.--active {
            --uui-button-background-color: var(--uui-color-current, #f5c1bc);
            --uui-button-background-color-hover: var(--uui-color-current-emphasis, #f8d6d3);
        }

    `;

}

customElements.define("limbo-youtube-button-list", LimboYouTubeButtonListElement);

export { LimboYouTubeButtonListElement as element };
export default LimboYouTubeButtonListElement;
