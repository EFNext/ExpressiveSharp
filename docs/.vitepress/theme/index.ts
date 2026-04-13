import DefaultTheme from 'vitepress/theme'
import ExpressiveSample from './components/ExpressiveSample.vue'
import './custom.css'

export default {
  ...DefaultTheme,
  enhanceApp({ app }) {
    app.component('ExpressiveSample', ExpressiveSample)
  }
}
