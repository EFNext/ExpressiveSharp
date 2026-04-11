import {defineConfig, type DefaultTheme, type HeadConfig} from 'vitepress'
import llmstxt from 'vitepress-plugin-llms'

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
        { text: '[Expressive] Properties', link: '/guide/expressive-properties' },
        { text: '[Expressive] Methods', link: '/guide/expressive-methods' },
        { text: 'Extension Members', link: '/guide/extension-members' },
        { text: 'Constructor Projections', link: '/guide/expressive-constructors' },
        { text: 'ExpressionPolyfill.Create', link: '/guide/expression-polyfill' },
        { text: 'IExpressiveQueryable<T>', link: '/guide/expressive-queryable' },
        { text: 'EF Core Integration', link: '/guide/ef-core-integration' },
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

export default defineConfig({
  title: "ExpressiveSharp",
  description: "Modern C# syntax in LINQ expression trees — source-generated at compile time",
  base,
  head: headers,
  themeConfig: {
    logo: '/logo.png',
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Guide', link: '/guide/introduction' },
      { text: 'Reference', link: '/reference/expressive-attribute' },
      { text: 'Advanced', link: '/advanced/how-it-works' },
      { text: 'Recipes', link: '/recipes/computed-properties' },
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
