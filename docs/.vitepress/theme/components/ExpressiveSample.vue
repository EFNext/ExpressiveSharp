<script setup lang="ts">
import { ref, computed } from 'vue'

interface Tab {
  id: string
  label: string
  html: string
  isError?: boolean
}

const props = defineProps<{
  csharpHtml: string   // base64-encoded pre-highlighted HTML
  tabsData: string     // base64-encoded JSON array of output tabs
  playgroundUrl: string
}>()

const csharpHtml = computed(() => {
  try { return atob(props.csharpHtml) } catch { return '' }
})

const tabs = computed<Tab[]>(() => {
  try { return JSON.parse(atob(props.tabsData)) } catch { return [] }
})

const activeTab = ref(0)
</script>

<template>
  <div class="expressive-sample">
    <!-- Input: always visible -->
    <div class="es-section">
      <div class="es-section-label">C#</div>
      <div class="es-code" v-html="csharpHtml"></div>
    </div>

    <!-- Output: tabbed -->
    <div class="es-section" v-if="tabs.length">
      <div class="es-tabs">
        <button
          v-for="(tab, i) in tabs"
          :key="tab.id"
          :class="{ active: activeTab === i }"
          @click="activeTab = i"
        >{{ tab.label }}</button>
      </div>
      <div v-for="(tab, i) in tabs" :key="tab.id" v-show="activeTab === i">
        <div v-if="tab.isError" class="es-error-banner">
          This query cannot be translated by <strong>{{ tab.label }}</strong>. The message from the query provider:
        </div>
        <div :class="['es-code', { 'es-code-error': tab.isError }]" v-html="tab.html"></div>
      </div>
    </div>

    <p class="es-footer">
      <a :href="playgroundUrl" class="es-playground-link">
        Open in Playground &rarr;
      </a>
    </p>
  </div>
</template>

<style scoped>
.expressive-sample {
  margin: 16px 0;
  border: 1px solid var(--vp-c-divider);
  border-radius: 8px;
  overflow: hidden;
}

.es-section + .es-section {
  border-top: 1px solid var(--vp-c-divider);
}

.es-section-label {
  padding: 6px 16px;
  font-size: 12px;
  font-weight: 600;
  color: var(--vp-c-text-2);
  background: var(--vp-code-tab-bg, var(--vp-c-bg-soft));
  text-transform: uppercase;
  letter-spacing: 0.04em;
}

.es-tabs {
  display: flex;
  flex-wrap: wrap;
  background: var(--vp-code-tab-bg, var(--vp-c-bg-soft));
  border-bottom: 1px solid var(--vp-c-divider);
}

.es-tabs button {
  padding: 8px 16px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  color: var(--vp-c-text-2);
  background: none;
  border: none;
  border-bottom: 2px solid transparent;
  transition: color 0.2s, border-color 0.2s;
  white-space: nowrap;
}

.es-tabs button:hover {
  color: var(--vp-c-text-1);
}

.es-tabs button.active {
  color: var(--vp-c-brand-1);
  border-bottom-color: var(--vp-c-brand-1);
}

.es-code {
  background: var(--vp-code-block-bg);
  overflow: hidden;
}

.es-error-banner {
  padding: 10px 16px;
  background: var(--vp-c-warning-soft, rgba(234, 179, 8, 0.14));
  color: var(--vp-c-warning-1, #b59400);
  border-bottom: 1px solid var(--vp-c-warning-2, rgba(234, 179, 8, 0.3));
  font-size: 13px;
}

.es-code-error :deep(pre.shiki-error) {
  margin: 0;
  padding: 16px 24px;
  font-family: var(--vp-font-family-mono);
  font-size: 13px;
  line-height: 1.6;
  color: var(--vp-c-text-2);
  background: transparent;
  white-space: pre-wrap;
  word-break: break-word;
}

.es-code :deep(pre.shiki) {
  margin: 0;
  padding: 16px 24px;
  overflow-x: auto;
  font-size: 13px;
  line-height: 1.6;
  background: transparent !important;
}

.es-code :deep(code) {
  font-family: var(--vp-font-family-mono);
  background: transparent;
}

/* Shiki dual-theme: switch between light & dark via CSS vars */
.es-code :deep(pre.shiki .line span) {
  color: var(--shiki-light);
  background-color: var(--shiki-light-bg) !important;
}

html.dark .es-code :deep(pre.shiki .line span) {
  color: var(--shiki-dark);
  background-color: var(--shiki-dark-bg) !important;
}

.es-footer {
  margin: 0;
  padding: 8px 16px;
  background: var(--vp-code-tab-bg, var(--vp-c-bg-soft));
  border-top: 1px solid var(--vp-c-divider);
}

.es-playground-link {
  font-size: 13px;
  font-weight: 500;
  color: var(--vp-c-brand-1);
  text-decoration: none;
}

.es-playground-link:hover {
  text-decoration: underline;
}
</style>
