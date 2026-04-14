import DefaultTheme from 'vitepress/theme'
import Layout from './Layout.vue'
import ExpressiveSample from './components/ExpressiveSample.vue'
import './custom.css'

export default {
  ...DefaultTheme,
  Layout,
  enhanceApp({ app }) {
    app.component('ExpressiveSample', ExpressiveSample)
  }
}
