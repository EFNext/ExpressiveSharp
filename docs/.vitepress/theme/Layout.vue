<script setup lang="ts">
import DefaultTheme from 'vitepress/theme'
import { useData } from 'vitepress'
import CopyPageButton from './components/CopyPageButton.vue'

const { frontmatter } = useData()
const { Layout } = DefaultTheme
</script>

<template>
  <Layout>
    <template
      v-if="frontmatter.layout !== 'home' && frontmatter.copyPage !== false"
      #doc-before
    >
      <CopyPageButton />
    </template>
  </Layout>
</template>
