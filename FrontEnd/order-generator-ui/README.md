# Order Generator UI

Interface React + TypeScript + Vite para geração de ordens do sistema OrderAccumulator.

## 🚀 Configuração

### Pré-requisitos

- Node.js 18+ 
- npm ou yarn

### Instalação

```bash
npm install
```

### Configuração da API

1. Copie o arquivo `.env.example` para `.env`:
   ```bash
   cp .env.example .env
   ```

2. Ajuste a URL da API no arquivo `.env` se necessário:
   ```env
   VITE_API_URL=https://localhost:7225
   ```

   **URLs disponíveis da API:**
   - HTTPS: `https://localhost:7225`
   - HTTP: `http://localhost:5287`

### Executando o projeto

```bash
# Modo desenvolvimento (com hot reload)
npm run dev

# Build para produção
npm run build

# Preview do build de produção
npm run preview
```

## 📁 Estrutura do Projeto

```
src/
├── components/     # Componentes React
├── services/       # Serviços de API
├── types/          # Definições TypeScript
├── App.tsx         # Componente principal
└── main.tsx        # Entry point
```

## 🔧 Tecnologias

- **React 19.2.5** - Biblioteca UI
- **TypeScript 6.0.2** - Tipagem estática
- **Vite 8.0.10** - Build tool e dev server
- **ESLint** - Linter

## 📝 Uso

A aplicação permite gerar ordens que são enviadas para a API do OrderAccumulator. As ordens são processadas via protocolo FIX e acumuladas conforme as regras de negócio.

---

# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```
