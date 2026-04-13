import {defineConfig, type DefaultTheme, type HeadConfig} from 'vitepress'
import llmstxt from 'vitepress-plugin-llms'
import {expressiveSamplePlugin} from './plugins/expressive-sample'
import {readFileSync, existsSync} from 'fs'
import {resolve, dirname} from 'path'
import {fileURLToPath} from 'url'
import {createHash} from 'crypto'

const __dirname = dirname(fileURLToPath(import.meta.url))

const base = '/ExpressiveSharp/'

const sidebar: DefaultTheme.Sidebar = {
  '/guide/': [
    {
      text: 'Getting Started',
      items: [
        { text: 'Introduction', link: '/guide/introduction' },
        { text: 'The Expression Tree Problem', link: '/guide/expression-tree-problem' },
        { text: 'Quick Start', link: '/guide/quickstart' },
      ]
    },
    {
      text: 'Core APIs',
      items: [
        { text: 'IExpressiveQueryable<T>', link: '/guide/expressive-queryable' },
        { text: '[Expressive] Properties', link: '/guide/expressive-properties' },
        { text: '[Expressive] Methods', link: '/guide/expressive-methods' },
        { text: 'Extension Members', link: '/guide/extension-members' },
        { text: 'Constructor Projections', link: '/guide/expressive-constructors' },
        { text: 'ExpressionPolyfill.Create', link: '/guide/expression-polyfill' },
      ]
    },
    {
      text: 'Integrations',
      items: [
        { text: 'EF Core', link: '/guide/integrations/ef-core' },
        { text: 'MongoDB', link: '/guide/integrations/mongodb' },
        { text: 'Custom Providers', link: '/guide/integrations/custom-providers' },
      ]
    },
    {
      text: 'Extensions',
      items: [
        { text: 'Window Functions (SQL)', link: '/guide/window-functions' },
      ]
    },
    {
      text: 'Migration',
      items: [
        { text: 'Migrating from Projectables', link: '/guide/migration-from-projectables' },
      ]
    }
  ],
  '/reference/': [
    {
      text: 'Reference',
      items: [
        { text: '[Expressive] Attribute', link: '/reference/expressive-attribute' },
        { text: 'Projectable Properties', link: '/reference/projectable-properties' },
        { text: '[ExpressiveFor] Mapping', link: '/reference/expressive-for' },
        { text: 'Null-Conditional Rewrite', link: '/reference/null-conditional-rewrite' },
        { text: 'Pattern Matching', link: '/reference/pattern-matching' },
        { text: 'Switch Expressions', link: '/reference/switch-expressions' },
        { text: 'Expression Transformers', link: '/reference/expression-transformers' },
        { text: 'Diagnostics & Code Fixes', link: '/reference/diagnostics' },
        { text: 'Troubleshooting', link: '/reference/troubleshooting' },
      ]
    }
  ],
  '/advanced/': [
    {
      text: 'Advanced',
      items: [
        { text: 'How It Works', link: '/advanced/how-it-works' },
        { text: 'IOperation to Expression Mapping', link: '/advanced/ioperation-mapping' },
        { text: 'Block-Bodied Members', link: '/advanced/block-bodied-members' },
        { text: 'Custom Transformers', link: '/advanced/custom-transformers' },
        { text: 'Testing Strategy', link: '/advanced/testing-strategy' },
        { text: 'Limitations', link: '/advanced/limitations' },
      ]
    }
  ],
  '/recipes/': [
    {
      text: 'Recipes',
      items: [
        { text: 'Computed Entity Properties', link: '/recipes/computed-properties' },
        { text: 'Projection Middleware (HotChocolate, AutoMapper)', link: '/recipes/projection-middleware' },
        { text: 'DTO Projections with Constructors', link: '/recipes/dto-projections' },
        { text: 'Scoring & Classification', link: '/recipes/scoring-classification' },
        { text: 'Nullable Navigation Properties', link: '/recipes/nullable-navigation' },
        { text: 'Reusable Query Filters', link: '/recipes/reusable-query-filters' },
        { text: 'External Member Mapping', link: '/recipes/external-member-mapping' },
        { text: 'Modern Syntax in LINQ Chains', link: '/recipes/modern-syntax-in-linq' },
        { text: 'Window Functions & Ranking', link: '/recipes/window-functions-ranking' },
      ]
    }
  ],
}

const umamiScript: HeadConfig = ["script", {
    defer: "true",
    src: "https://cloud.umami.is/script.js",
    "data-website-id": "TBD",
}]

const baseHeaders: HeadConfig[] = [
    ['link', { rel: 'icon', type: 'image/png', href: `${base}logo.png` }],
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { name: 'twitter:card', content: 'summary_large_image' }],
];

const headers = process.env.GITHUB_ACTIONS === "true" ?
    [...baseHeaders, umamiScript] :
    baseHeaders;

// Vite plugin: serve _playground/app.htm as raw HTML in dev mode.
// VitePress's dev server applies its SPA transform to all HTML files in
// public/, which breaks the Blazor WASM app. This middleware intercepts
// requests to _playground/app.htm and serves the raw file directly.
const mimeTypes: Record<string, string> = {
  '.htm': 'text/html', '.html': 'text/html', '.js': 'application/javascript',
  '.mjs': 'application/javascript', '.css': 'text/css', '.json': 'application/json',
  '.wasm': 'application/wasm', '.dll': 'application/octet-stream',
  '.dat': 'application/octet-stream', '.br': 'application/octet-stream',
  '.gz': 'application/octet-stream', '.woff': 'font/woff', '.woff2': 'font/woff2',
}

// Expands `::: expressive-sample` containers into fenced code blocks for each
// render target BEFORE VitePress or llmstxt sees the markdown. This way:
//   - llms.txt sees the actual SQL / MongoDB / generator output
//   - VitePress renders the fenced blocks as regular code blocks (with Shiki
//     highlighting) which our markdown-it plugin picks up and wraps as tabs
// The fenced blocks are the single source of truth the Vue component reads
// from via the `data-expressive-sample` marker injected on the first block.
function expandExpressiveSamplesPlugin() {
  return {
    name: 'expand-expressive-samples',
    enforce: 'pre' as const,
    transform(code: string, id: string) {
      if (!id.endsWith('.md')) return null
      if (!code.includes('::: expressive-sample')) return null

      const relPath = id.includes('/docs/')
        ? id.substring(id.indexOf('/docs/') + 6).replace(/\?.*$/, '')
        : id
      const jsonPath = resolve(__dirname, 'data/samples', relPath.replace(/\.md$/, '.json'))
      if (!existsSync(jsonPath)) return null

      type Target = { label: string; language: string; output: string }
      type Sample = { key: string; snippet: string; setup?: string | null; targets: Record<string, Target> }
      let samples: Sample[]
      try { samples = JSON.parse(readFileSync(jsonPath, 'utf-8')) } catch { return null }

      const lines = code.split('\n')
      const result: string[] = []
      let i = 0
      while (i < lines.length) {
        if (!lines[i].trimStart().startsWith('::: expressive-sample')) {
          result.push(lines[i]); i++; continue
        }
        i++
        const bodyLines: string[] = []
        while (i < lines.length && lines[i].trimStart() !== ':::') {
          bodyLines.push(lines[i]); i++
        }
        i++ // closing :::

        const body = bodyLines.join('\n').trim()
        const sepIdx = body.indexOf('---setup---')
        const snippet = sepIdx >= 0 ? body.slice(0, sepIdx).trim() : body
        const setup = sepIdx >= 0 ? body.slice(sepIdx + '---setup---'.length).trim() : undefined

        const key = createHash('sha256')
          .update(snippet + '\0' + (setup ?? ''))
          .digest('hex').slice(0, 12).toLowerCase()
        const sample = samples.find(s => s.key === key)
        if (!sample) {
          // Fallback: leave the container for our markdown-it plugin's warning
          result.push('::: expressive-sample')
          result.push(...bodyLines)
          result.push(':::')
          continue
        }

        // Preserve original container — our markdown-it plugin (VitePress
        // render stage) reads this and emits the interactive Vue tabs.
        result.push('::: expressive-sample')
        result.push(...bodyLines)
        result.push(':::')

        // Also emit fenced code blocks inside a hidden div. These are invisible
        // on the rendered page (Vue component handles the UI) but are included
        // in the raw .md that llms.txt sees, so crawlers/LLMs get the full SQL
        // and pipeline output for each render target.
        result.push('')
        result.push('<div class="expressive-sample-llms" style="display:none">')
        result.push('')
        // For LLMs: include C# input and ONE representative SQL output (SQLite).
        // The other providers are mostly SQL-dialect noise that doesn't teach
        // anything about ExpressiveSharp; the generator output is boilerplate
        // that shouldn't influence LLM suggestions toward [InterceptsLocation].
        let csharpContent = sample.snippet
        if (sample.setup) csharpContent += '\n\n// Setup\n' + sample.setup
        result.push('```csharp')
        result.push(csharpContent)
        result.push('```')
        const sqlite = sample.targets['sqlite']
        if (sqlite) {
          result.push('')
          result.push(`**Generated SQL:**`)
          result.push('')
          result.push('```' + sqlite.language)
          result.push(sqlite.output)
          result.push('```')
        }
        result.push('')
        result.push('</div>')
        result.push('')
      }
      return { code: result.join('\n'), map: null }
    }
  }
}

function servePlaygroundPlugin() {
  return {
    name: 'serve-playground',
    configureServer(server: any) {
      // Serve everything under /_playground/ as raw static files so VitePress's
      // SPA transform and module system don't intercept Blazor WASM resources.
      server.middlewares.use((req: any, res: any, next: any) => {
        const prefix = '/ExpressiveSharp/_playground/'
        if (!req.url?.startsWith(prefix)) return next()

        const relPath = req.url.slice(prefix.length).split('?')[0]
        const filePath = resolve(__dirname, '../public/_playground', relPath)
        if (!existsSync(filePath)) return next()

        const ext = '.' + relPath.split('.').pop()
        res.setHeader('Content-Type', mimeTypes[ext] || 'application/octet-stream')
        res.end(readFileSync(filePath))
      })
    }
  }
}

export default defineConfig({
  title: "ExpressiveSharp",
  description: "Modern C# syntax in LINQ expression trees — source-generated at compile time",
  base,
  head: headers,
  markdown: {
    config: (md) => {
      md.use(expressiveSamplePlugin)
    }
  },
  vue: {
    template: {
      compilerOptions: {
        isCustomElement: (tag) => tag === 'expressive-playground',
      }
    }
  },
  themeConfig: {
    logo: '/logo.png',
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Guide', link: '/guide/introduction' },
      { text: 'Reference', link: '/reference/expressive-attribute' },
      { text: 'Advanced', link: '/advanced/how-it-works' },
      { text: 'Recipes', link: '/recipes/computed-properties' },
      { text: 'Playground', link: '/playground-editor' },
      { text: 'Benchmarks', link: 'https://efnext.github.io/ExpressiveSharp/dev/bench/' },
    ],

    sidebar,

    socialLinks: [
      { icon: 'github', link: 'https://github.com/EFNext/ExpressiveSharp' }
    ],

    search: {
      provider: 'local'
    },

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Copyright &copy; ExpressiveSharp Contributors'
    }
  },
  vite: {
    plugins: [
      expandExpressiveSamplesPlugin(),
      servePlaygroundPlugin(),
      llmstxt({
        domain: 'https://efnext.github.io',
        description: 'Modern C# syntax in LINQ expression trees — source-generated at compile time',
        details:
          'ExpressiveSharp is a Roslyn source generator that enables modern C# syntax ' +
          '(null-conditional ?., switch expressions, pattern matching) inside LINQ expression trees. ' +
          'It emits Expression<TDelegate> factory code at compile time from [Expressive]-decorated ' +
          'members and integrates with EF Core, MongoDB, and any IQueryable provider.',
        sidebar,
      }),
    ],
  },
})
