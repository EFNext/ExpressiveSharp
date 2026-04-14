<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useData, withBase } from 'vitepress'

const { page } = useData()

const open = ref(false)
const copied = ref<string | null>(null)
const root = ref<HTMLElement | null>(null)

// withBase prepends site base (e.g. "/ExpressiveSharp/") and ensures a leading slash.
const mdUrl = computed(() => withBase('/' + page.value.relativePath))
const llmUrl = computed(() => withBase('/' + page.value.relativePath.replace(/\.md$/, '.llm.md')))

const absoluteUrl = computed(() => {
  if (typeof window === 'undefined') return ''
  return window.location.origin + window.location.pathname
})

const llmPrompt = computed(() =>
  encodeURIComponent(
    `Read ${absoluteUrl.value} and help me with ExpressiveSharp.`
  )
)

async function copyFromUrl(url: string, label: string) {
  try {
    console.log('[CopyPage] fetching', url)
    const res = await fetch(url)
    console.log('[CopyPage] response', res.status, res.headers.get('content-type'))
    if (!res.ok) throw new Error(`${res.status}`)
    const text = await res.text()
    console.log('[CopyPage] body starts with:', text.slice(0, 80))
    await navigator.clipboard.writeText(text)
    copied.value = label
    setTimeout(() => { if (copied.value === label) copied.value = null }, 1500)
  } catch (e) {
    console.error('[CopyPage] failed', e)
    copied.value = 'Copy failed'
    setTimeout(() => { if (copied.value === 'Copy failed') copied.value = null }, 1500)
  }
  open.value = false
}

function copyMarkdown() { copyFromUrl(mdUrl.value, 'Copied!') }
function copyMarkdownExpanded() { copyFromUrl(llmUrl.value, 'Copied!') }

function viewMarkdown() {
  window.open(mdUrl.value, '_blank', 'noopener')
  open.value = false
}

function openInChatGpt() {
  window.open(`https://chat.openai.com/?q=${llmPrompt.value}`, '_blank', 'noopener')
  open.value = false
}

function openInClaude() {
  window.open(`https://claude.ai/new?q=${llmPrompt.value}`, '_blank', 'noopener')
  open.value = false
}

function onDocClick(e: MouseEvent) {
  if (!root.value) return
  if (!root.value.contains(e.target as Node)) open.value = false
}

onMounted(() => document.addEventListener('click', onDocClick))
onUnmounted(() => document.removeEventListener('click', onDocClick))
</script>

<template>
  <div class="copy-page" ref="root">
    <div class="copy-page-group">
      <button class="copy-page-main" @click="copyMarkdown" :title="'Copy page as markdown'">
        <svg class="icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>
          <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path>
        </svg>
        <span>{{ copied ?? 'Copy page' }}</span>
      </button>
      <button class="copy-page-chevron" @click="open = !open" :aria-expanded="open" :title="'More actions'">
        <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="6 9 12 15 18 9"></polyline>
        </svg>
      </button>
    </div>
    <div v-if="open" class="copy-page-menu" role="menu">
      <button role="menuitem" @click="copyMarkdown">
        <span class="menu-title">Copy as Markdown</span>
        <span class="menu-hint">Raw page source</span>
      </button>
      <button role="menuitem" @click="copyMarkdownExpanded">
        <span class="menu-title">Copy with Rendered SQL</span>
        <span class="menu-hint">Live samples expanded inline</span>
      </button>
      <button role="menuitem" @click="viewMarkdown">
        <span class="menu-title">View as Markdown ↗</span>
        <span class="menu-hint">Open raw .md in new tab</span>
      </button>
      <div class="menu-divider"></div>
      <button role="menuitem" @click="openInChatGpt">
        <span class="menu-title">Open in ChatGPT ↗</span>
        <span class="menu-hint">Ask about this page</span>
      </button>
      <button role="menuitem" @click="openInClaude">
        <span class="menu-title">Open in Claude ↗</span>
        <span class="menu-hint">Ask about this page</span>
      </button>
    </div>
  </div>
</template>

<style scoped>
.copy-page {
  position: relative;
  display: flex;
  justify-content: flex-end;
  align-items: stretch;
  margin: 0 0 12px;
  font-size: 12px;
  font-weight: 500;
  z-index: 20;
}

.copy-page-group {
  display: inline-flex;
  align-items: stretch;
}

.copy-page-main,
.copy-page-chevron {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  background: var(--vp-c-bg-soft);
  color: var(--vp-c-text-2);
  border: 1px solid var(--vp-c-divider);
  cursor: pointer;
  transition: color 0.15s, background 0.15s, border-color 0.15s;
}

.copy-page-main {
  border-radius: 6px 0 0 6px;
  border-right-width: 0;
}

.copy-page-chevron {
  border-radius: 0 6px 6px 0;
  padding: 4px 6px;
}

.copy-page-main:hover,
.copy-page-chevron:hover,
.copy-page-chevron[aria-expanded="true"] {
  color: var(--vp-c-text-1);
  background: var(--vp-c-bg);
  border-color: var(--vp-c-brand-1);
}

.copy-page-main .icon {
  opacity: 0.75;
}

.copy-page-menu {
  position: absolute;
  top: calc(100% + 6px);
  right: 0;
  min-width: 240px;
  padding: 6px;
  background: var(--vp-c-bg-elv, var(--vp-c-bg));
  border: 1px solid var(--vp-c-divider);
  border-radius: 8px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.18);
  z-index: 50;
}

.copy-page-menu button {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  width: 100%;
  padding: 6px 10px;
  background: transparent;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  text-align: left;
  color: var(--vp-c-text-1);
}

.copy-page-menu button:hover {
  background: var(--vp-c-bg-soft);
}

.copy-page-menu .menu-title {
  font-size: 13px;
  font-weight: 500;
}

.copy-page-menu .menu-hint {
  font-size: 11px;
  color: var(--vp-c-text-3);
  margin-top: 1px;
}

.copy-page-menu .menu-divider {
  height: 1px;
  margin: 4px 0;
  background: var(--vp-c-divider);
}
</style>
