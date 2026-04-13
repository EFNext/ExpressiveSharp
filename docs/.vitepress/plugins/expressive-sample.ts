import type MarkdownIt from 'markdown-it'
import { createHash } from 'crypto'
import { readFileSync, existsSync } from 'fs'
import { resolve } from 'path'
import { createHighlighterCoreSync } from 'shiki/core'
import { createJavaScriptRegexEngine } from '@shikijs/engine-javascript'
import csharp from 'shiki/langs/csharp.mjs'
import sql from 'shiki/langs/sql.mjs'
import javascript from 'shiki/langs/javascript.mjs'
import githubLight from 'shiki/themes/github-light.mjs'
import githubDark from 'shiki/themes/github-dark.mjs'

interface RenderedTarget {
  label: string
  language: string
  output: string
  isError?: boolean
}

interface RenderedSample {
  key: string
  snippet: string
  setup?: string | null
  targets: Record<string, RenderedTarget>
}

interface HighlightedTab {
  id: string
  label: string
  html: string  // pre-highlighted HTML (dual light/dark via Shiki)
  isError?: boolean
}

const SETUP_SEPARATOR = '---setup---'
const SAMPLES_DIR = resolve(__dirname, '../data/samples')
const BASE_PATH = '/ExpressiveSharp/'

const fileCache = new Map<string, RenderedSample[]>()

// Single shared Shiki highlighter. Sync engine (JS regex) so we can call it
// from the markdown-it plugin's synchronous parse hook.
const highlighter = createHighlighterCoreSync({
  themes: [githubLight, githubDark],
  langs: [csharp, sql, javascript],
  engine: createJavaScriptRegexEngine(),
})

function highlight(code: string, lang: string): string {
  const normalized = lang === 'plaintext' || !['csharp', 'sql', 'javascript'].includes(lang)
    ? 'plaintext' : lang
  try {
    return highlighter.codeToHtml(code, {
      lang: normalized === 'plaintext' ? 'csharp' : normalized,
      themes: { light: 'github-light', dark: 'github-dark' },
      defaultColor: false,  // emits CSS vars so VitePress dark mode toggle works
    })
  } catch {
    // Fallback to escaped plain text
    const escaped = code
      .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
    return `<pre class="shiki"><code>${escaped}</code></pre>`
  }
}

function loadSamplesForFile(mdRelativePath: string): RenderedSample[] {
  const jsonPath = resolve(SAMPLES_DIR, mdRelativePath.replace(/\.md$/, '.json'))
  if (fileCache.has(jsonPath)) return fileCache.get(jsonPath)!
  if (!existsSync(jsonPath)) { fileCache.set(jsonPath, []); return [] }
  try {
    const data = JSON.parse(readFileSync(jsonPath, 'utf-8')) as RenderedSample[]
    fileCache.set(jsonPath, data)
    return data
  } catch {
    fileCache.set(jsonPath, [])
    return []
  }
}

function computeStableKey(snippet: string, setup?: string | null): string {
  const input = snippet + '\0' + (setup ?? '')
  return createHash('sha256').update(input).digest('hex').slice(0, 12).toLowerCase()
}

function escapeAttr(str: string): string {
  return str.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
}

function buildPlaygroundUrl(snippet: string, setup?: string | null): string {
  const params = new URLSearchParams()
  params.set('snippet', Buffer.from(snippet).toString('base64url'))
  if (setup) params.set('setup', Buffer.from(setup).toString('base64url'))
  params.set('scenario', 'webshop')
  return `${BASE_PATH}playground-editor#${params.toString()}`
}

export function expressiveSamplePlugin(md: MarkdownIt): void {
  const originalParse = md.parse.bind(md)

  md.parse = function (src: string, env: any): any[] {
    const relativePath: string | undefined = env?.relativePath
    if (!relativePath) return originalParse(src, env)

    const samples = loadSamplesForFile(relativePath)
    const lines = src.split('\n')
    const result: string[] = []
    let i = 0

    while (i < lines.length) {
      const trimmed = lines[i].trimStart()
      if (!trimmed.startsWith('::: expressive-sample')) {
        result.push(lines[i])
        i++
        continue
      }

      i++
      const bodyLines: string[] = []
      while (i < lines.length) {
        const closeTrimmed = lines[i].trimStart()
        if (closeTrimmed === ':::') break
        bodyLines.push(lines[i])
        i++
      }
      i++ // skip closing :::

      const body = bodyLines.join('\n').trim()
      const sepIdx = body.indexOf(SETUP_SEPARATOR)

      let snippet: string
      let setup: string | undefined

      if (sepIdx >= 0) {
        snippet = body.slice(0, sepIdx).trim()
        setup = body.slice(sepIdx + SETUP_SEPARATOR.length).trim()
      } else {
        snippet = body
      }

      const key = computeStableKey(snippet, setup)
      const sample = samples.find(s => s.key === key)

      if (sample) {
        // C# code (snippet + optional setup) — pre-highlighted, always visible
        let csharpContent = sample.snippet
        if (sample.setup) {
          csharpContent += '\n\n// Setup\n' + sample.setup
        }
        const csharpHtml = highlight(csharpContent, 'csharp')

        // Output tabs
        const outputTabs: HighlightedTab[] = []
        for (const targetId of ['sqlite', 'postgres', 'sqlserver', 'cosmos', 'mongodb', 'generator']) {
          const target = sample.targets[targetId]
          if (target) {
            outputTabs.push({
              id: targetId,
              label: target.label,
              html: target.isError
                ? `<pre class="shiki-error"><code>${target.output.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')}</code></pre>`
                : highlight(target.output, target.language),
              isError: target.isError,
            })
          }
        }

        const playgroundUrl = buildPlaygroundUrl(snippet, setup)
        const tabsBase64 = Buffer.from(JSON.stringify(outputTabs)).toString('base64')
        const csharpBase64 = Buffer.from(csharpHtml).toString('base64')

        result.push('')
        result.push(
          `<ExpressiveSample csharpHtml="${csharpBase64}" tabsData="${tabsBase64}" playgroundUrl="${escapeAttr(playgroundUrl)}" />`
        )
        result.push('')
      } else {
        result.push('')
        result.push('::: warning Pre-rendered output not available')
        result.push(`Run the pre-renderer to generate output for this sample (key: \`${key}\`).`)
        result.push(':::')
        result.push('')
        result.push('```csharp')
        result.push(snippet)
        result.push('```')
        result.push('')
      }
    }

    return originalParse(result.join('\n'), env)
  }
}
