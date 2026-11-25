# Próximos Passos — Sudoku Verde

## 1. Status Atual
- **Variação A** e **Variação B** implementadas em Unity UI Toolkit compartilhando `Core/` e `UI/Shared/`, com seletor A/B funcional e persistente em Configurações.

## 2. Realização dos Testes com o Público
- Aplicar o roteiro presencial descrito em `TESTE_AB.md` com participantes 60+ alternando a ordem das variantes (A→B e B→A) em proporção vertical (`400 × 800`).
- Registrar manualmente dificuldades de leitura, compreensão das regras, erros de clique duplo e fluxo entre telas.

## 3. Análise dos Feedbacks
- Consolidar acertos e pontos de atrito das Variações A e B na tabela comparativa de `TESTE_AB.md`.
- Priorizar melhorias com base nos erros recorrentes observados durante as tarefas.

## 4. Melhorias a Partir dos Resultados
- Refinar tamanhos de fonte, rótulos do botão de alternância `Planta / X` e feedback visual de conflito conforme retorno dos participantes.
- Incorporar a fonte licenciada `Winkle-Regular` (caso disponibilizada para a entrega final) no lugar de `Lato-Bold`.

## 5. Pendências da Implementação Atual
- **Validação em PlayMode na Unity:** Conforme política `UNITY_EXECUTION=MANUAL_ONLY`, a compilação C#, o preview no UI Builder e os testes interativos no Unity Editor `6000.0.35f1` devem ser executados manualmente pelo usuário no Editor aberto.
- **Integração de áudio e háptica:** Caso seja desejado efeito sonoro real para `MUSICA` / `VOZ` e vibração física para `VIBRA`, conectar clipes de áudio e `Handheld.Vibrate()` (mobile) aos eventos do `SettingsViewModel`.
