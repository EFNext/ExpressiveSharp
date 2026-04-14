---
layout: page
title: Playground
---

<style>
  .VPDoc .container { max-width: 100%; }
  .VPDoc .content-container { max-width: 100%; padding: 0; }
  .VPDoc .aside { display: none; }
  .playground-iframe {
    width: 100%;
    border: 0;
    display: block;
    min-height: 500px;
  }
</style>

<iframe class="playground-iframe" id="playground-frame" title="ExpressiveSharp Playground"></iframe>

<script setup>
import { onMounted, onUnmounted } from 'vue'

function isDark() {
  return document.documentElement.classList.contains('dark')
}

let resizeHandler = null
let observer = null

onMounted(() => {
  const frame = document.getElementById('playground-frame')
  if (!frame) return

  const base = location.origin + '/ExpressiveSharp/_playground/app.htm'
  const theme = isDark() ? 'dark' : 'light'
  const hash = location.hash || ''
  frame.setAttribute('src', `${base}?theme=${theme}${hash}`)

  resizeHandler = (e) => {
    if (e.data?.type === 'playground-resize') {
      frame.style.height = e.data.height + 'px'
    }
  }
  window.addEventListener('message', resizeHandler)

  observer = new MutationObserver(() => {
    if (frame.contentWindow) {
      frame.contentWindow.postMessage(isDark() ? 'theme:dark' : 'theme:light', '*')
    }
  })
  observer.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] })
})

onUnmounted(() => {
  if (resizeHandler) window.removeEventListener('message', resizeHandler)
  if (observer) observer.disconnect()
})
</script>
