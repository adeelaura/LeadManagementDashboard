(() => {
    "use strict";

    const ready = (callback) => {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", callback, { once: true });
        } else {
            callback();
        }
    };

    ready(() => {
        document.querySelectorAll('.toast[data-auto-show="true"]').forEach((element) => {
            bootstrap.Toast.getOrCreateInstance(element).show();
        });

        document.querySelectorAll("form[data-confirm]").forEach((form) => {
            form.addEventListener("submit", (event) => {
                const message = form.dataset.confirm;
                if (message && !window.confirm(message)) {
                    event.preventDefault();
                }
            });
        });

        document.querySelectorAll("form").forEach((form) => {
            form.addEventListener("submit", () => {
                if (!form.checkValidity()) {
                    return;
                }

                const submitButton = form.querySelector('button[type="submit"]');
                if (submitButton && !submitButton.disabled) {
                    submitButton.classList.add("is-submitting");
                    submitButton.setAttribute("aria-busy", "true");
                }
            });
        });

        initializeLeadDetailsPanel();
        initializeStatusColorFields();
    });

    function initializeLeadDetailsPanel() {
        const panel = document.getElementById("leadDetailsPanel");
        const content = document.getElementById("leadDetailsContent");
        if (!panel || !content) {
            return;
        }

        const offcanvas = bootstrap.Offcanvas.getOrCreateInstance(panel);

        document.addEventListener("click", async (event) => {
            const trigger = event.target.closest("[data-lead-details-url]");
            if (!trigger) {
                return;
            }

            event.preventDefault();
            const url = trigger.dataset.leadDetailsUrl;
            if (!url) {
                return;
            }

            content.innerHTML = `
                <div class="details-loading" role="status">
                    <div class="spinner-border text-primary" aria-hidden="true"></div>
                    <span>Loading lead details…</span>
                </div>`;
            offcanvas.show();

            try {
                const response = await fetch(url, {
                    method: "GET",
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });

                if (!response.ok) {
                    throw new Error(`Request failed with status ${response.status}`);
                }

                content.innerHTML = await response.text();
            } catch (error) {
                console.error("Unable to load lead details.", error);
                content.innerHTML = `
                    <div class="alert alert-danger" role="alert">
                        <strong>Unable to load this lead.</strong>
                        <div class="mt-1">Close the panel and try again.</div>
                    </div>`;
            }
        });
    }

    function initializeStatusColorFields() {
        document.querySelectorAll("form").forEach((form) => {
            const picker = form.querySelector("[data-color-picker]");
            const code = form.querySelector("[data-color-code]");
            const preview = form.querySelector("[data-status-preview]");
            const nameInput = form.querySelector('input[name$=".Name"]');
            const previewName = form.querySelector("[data-status-preview-name]");

            if (!picker || !code) {
                return;
            }

            const validHex = /^#[0-9a-fA-F]{6}$/;

            const updatePreview = (value) => {
                if (!validHex.test(value)) {
                    return;
                }

                const normalized = value.toUpperCase();
                picker.value = normalized;
                code.value = normalized;
                if (preview) {
                    preview.style.setProperty("--preview-color", normalized);
                }
            };

            picker.addEventListener("input", () => updatePreview(picker.value));
            code.addEventListener("input", () => updatePreview(code.value.trim()));

            if (nameInput && previewName) {
                nameInput.addEventListener("input", () => {
                    previewName.textContent = nameInput.value.trim() || "New status preview";
                });
            }
        });
    }
})();
