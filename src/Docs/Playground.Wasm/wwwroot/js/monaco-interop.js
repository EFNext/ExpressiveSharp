// monaco-interop.js — thin JSInterop bridge between Blazor and Monaco editor.
// Replaces BlazorMonaco by calling Monaco's API directly. This avoids the
// BlazorMonaco dependency which hardcodes document.baseURI for AMD loader
// paths, preventing the playground from being hosted as a web component on
// a page with a different base URL.

const editors = {};
const editorCallbacks = {};

window.monacoInterop = {

    // ─── Editor lifecycle ───────────────────────────────────────────

    async create(elementId, options, dotnetRef) {
        // Wait for Monaco AMD require() to finish
        if (window.__monacoReady) await window.__monacoReady;

        const container = document.getElementById(elementId);
        if (!container) { console.warn('[monaco-interop] container not found:', elementId); return; }
        console.log('[monaco-interop] create', elementId, container.offsetWidth + 'x' + container.offsetHeight, options);

        const editor = monaco.editor.create(container, {
            language: options.language || 'plaintext',
            value: options.value || '',
            readOnly: options.readOnly || false,
            automaticLayout: true,
            minimap: { enabled: false },
            fontSize: options.fontSize || 13,
            scrollBeyondLastLine: false,
            lineNumbers: options.lineNumbers || 'off',
            folding: false,
            glyphMargin: false,
            lineDecorationsWidth: options.lineDecorationsWidth || 4,
            wordWrap: options.wordWrap || 'off',
            renderLineHighlight: options.renderLineHighlight || 'line',
            scrollbar: options.scrollbar || {},
        });

        editors[elementId] = editor;

        if (dotnetRef) {
            editor.onDidChangeModelContent(() => {
                dotnetRef.invokeMethodAsync('OnContentChanged');
            });
        }

        return editor.getModel()?.uri?.toString() || null;
    },

    dispose(elementId) {
        const editor = editors[elementId];
        if (editor) {
            editor.dispose();
            delete editors[elementId];
        }
    },

    // ─── Editor operations ──────────────────────────────────────────

    getValue(elementId) {
        return editors[elementId]?.getValue() || '';
    },

    setValue(elementId, value) {
        const editor = editors[elementId];
        if (editor) editor.setValue(value);
    },

    getModelUri(elementId) {
        return editors[elementId]?.getModel()?.uri?.toString() || null;
    },

    setModelLanguage(elementId, language) {
        const editor = editors[elementId];
        if (editor) {
            const model = editor.getModel();
            if (model) monaco.editor.setModelLanguage(model, language);
        }
    },

    // ─── Markers (squiggles) ────────────────────────────────────────

    setModelMarkers(elementId, owner, markers) {
        const editor = editors[elementId];
        if (!editor) return;
        const model = editor.getModel();
        if (!model) return;
        monaco.editor.setModelMarkers(model, owner, markers);
    },

    // ─── Language providers ─────────────────────────────────────────

    registerCompletionProvider(dotnetRef) {
        monaco.languages.registerCompletionItemProvider('csharp', {
            triggerCharacters: ['.', ' '],
            provideCompletionItems: async (model, position) => {
                const result = await dotnetRef.invokeMethodAsync(
                    'ProvideCompletionItems',
                    model.uri.toString(),
                    { lineNumber: position.lineNumber, column: position.column }
                );
                if (!result) return { suggestions: [] };
                // Map the suggestions to Monaco format
                return {
                    suggestions: result.suggestions.map(s => ({
                        label: s.label,
                        kind: s.kind,
                        insertText: s.insertText,
                        sortText: s.sortText || s.label,
                        filterText: s.filterText || s.label,
                        detail: s.detail || '',
                        range: s.range ? {
                            startLineNumber: s.range.startLineNumber,
                            startColumn: s.range.startColumn,
                            endLineNumber: s.range.endLineNumber,
                            endColumn: s.range.endColumn,
                        } : undefined,
                    })),
                    incomplete: result.incomplete || false,
                };
            },
        });
    },

    registerHoverProvider(dotnetRef) {
        monaco.languages.registerHoverProvider('csharp', {
            provideHover: async (model, position) => {
                const result = await dotnetRef.invokeMethodAsync(
                    'ProvideHover',
                    model.uri.toString(),
                    { lineNumber: position.lineNumber, column: position.column }
                );
                if (!result) return null;
                return {
                    contents: (result.contents || []).map(c => ({
                        value: c.value,
                        isTrusted: c.isTrusted || false,
                    })),
                    range: result.range ? {
                        startLineNumber: result.range.startLineNumber,
                        startColumn: result.range.startColumn,
                        endLineNumber: result.range.endLineNumber,
                        endColumn: result.range.endColumn,
                    } : undefined,
                };
            },
        });
    },

    // ─── Theme ──────────────────────────────────────────────────────

    setTheme(themeName) {
        monaco.editor.setTheme(themeName);
    },
};
